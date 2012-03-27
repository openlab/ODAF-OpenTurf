using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using ODAF.Data;
using website_mvc.Code;

namespace vancouveropendata
{

    /// <summary>
    /// Implements the search functionality using Lucene.net
    /// </summary>
    public sealed class SearchEngine
    {

        Object _ReadLocker = new Object();
        Object _WriteLocker = new Object();
        private IndexSearcher _Searcher1 = null;
        private IndexSearcher _Searcher2 = null;
        private DateTime _CreatedOn;
        private Directory _Directory;
        private CloudQueue _CloudQueue;

        SearchEngine()
        {
            // Instantiate the AzureDirectory storage object for use by Lucene
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                configSetter(CloudSettingsResolver.GetConfigSetting(configName));
            });
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.FromConfigurationSetting("BlobStorageEndpoint");
            _Directory = new AzureDirectory(cloudStorageAccount, "LuceneStorage", new RAMDirectory());

            // Create search index queue
            CloudQueueClient client = cloudStorageAccount.CreateCloudQueueClient();
            _CloudQueue = client.GetQueueReference("searchindexqueue");
            _CloudQueue.CreateIfNotExist();

        }

        #region singleton

        static readonly SearchEngine instance = new SearchEngine();

        static SearchEngine()
        {
            
        }

        

        public static SearchEngine Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        #region index

        public void Index(long summaryId)
        {
            Index(new List<long>() { summaryId });
        }

        public void Index(List<long> summaryIds)
        {
            foreach (var summaryId in summaryIds)
            {
                _CloudQueue.BeginAddMessage(new CloudQueueMessage(summaryId.ToString()), null, null);
            }
        }

        public void RemoveFromIndex(long summaryId)
        {
            Index(new List<long>() { -summaryId });
        }

        public void RemoveFromIndex(List<long> summaryIds)
        {
            foreach (var summaryId in summaryIds)
            {
                _CloudQueue.BeginAddMessage(new CloudQueueMessage((-summaryId).ToString()), null, null);
            }
        }

        #endregion

        #region search

        /// <summary>
        /// Retrieves the IndexSearcher.  As Lucene uses a ramdisk for reads, newly indexed
        /// documents won't show up in the results until a new IndexSearcher is instantiated.
        /// Due to this sync issue the index searcher is re-instantiated every 5 minutes.
        /// </summary>
        /// <returns></returns>
        private IndexSearcher GetIndexSearcher()
        {
            if (_Searcher2 != null) return _Searcher2;
            if (_Searcher1 == null || _CreatedOn < DateTime.Now.AddMinutes(-5))
            {
                lock (_ReadLocker)
                {
                    if (_Searcher1 == null || _CreatedOn < DateTime.Now.AddMinutes(-5))
                    {
                        try
                        {
                            _Searcher2 = _Searcher1;
                            _Searcher1 = new IndexSearcher(_Directory, true);
                            _CreatedOn = DateTime.Now;
                        }
                        finally
                        {
                            _Searcher2 = null;
                        }
                    }
                }
            }
            return _Searcher1;
        }

        /// <summary>
        ///  Returns PointDataSummary object list of summaries that match the specified lucene search phrase.
        /// </summary>
        /// <param name="phrase">Lucene search phrase.</param>
        /// <param name="maxResults">Search results to return.  Default and max is 100.</param>
        /// <returns></returns>
        public IList<PointDataSummary> SearchForPhrase(string phrase, int maxResults)
        {
            phrase = phrase.Trim();
            if (maxResults <= 0 || maxResults > 100)
                maxResults = 100;

            if (!string.IsNullOrEmpty(phrase))
            {

                // Construct a lucene query object
                Lucene.Net.QueryParsers.MultiFieldQueryParser parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, new string[] { "Tags", "Name" }, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29));
                Lucene.Net.Search.Query query = parser.Parse(phrase);
                
                // Pass the search to the IndexSearcher to query the Lucene index
                IndexSearcher indexSearcher = GetIndexSearcher();
                TopDocs docs = indexSearcher.Search(query, null, maxResults);
                if (docs.totalHits > 0)
                {
                    // Retrieve the point_id's from Lucene, then retrieve the PointDataSummary objects from the
                    // datastore, keeping lucene sort order.
                    Dictionary<long, PointDataSummary> sortedIdDictionary = docs.scoreDocs
                        .Select(p => new { id = long.Parse(indexSearcher.Doc(p.doc).Get("point_id")), val = new PointDataSummary() })
                        .ToDictionary(p => p.id, v => v.val);
                    IList<PointDataSummary> summaries = PointDataSummary.Find(p => sortedIdDictionary.Keys.Any(v => v == p.Id));
                    foreach (PointDataSummary summary in summaries) sortedIdDictionary[summary.Id] = summary;
                    return (IList<PointDataSummary>)sortedIdDictionary.Values.ToList();
                }
            }
            return new List<PointDataSummary>();
        }

        #endregion

    }

}
