using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Store.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using ODAF.Data;
using System.Diagnostics;
using System.Collections.Generic;
using website_mvc.Code;

namespace ODAF.SearchIndex
{
    public class WorkerRole : RoleEntryPoint
    {

        const int _SleepInMinutes = 1;
        private Directory _Directory;

        public override void Run()
        {

            Trace.WriteLine(DateTime.Now.ToString() + " [INIT]");

            // Create azure account credentials
            CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
            {
                configSetter(CloudSettingsResolver.GetConfigSetting(configName));
            });
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.FromConfigurationSetting("BlobStorageEndpoint");

            // Create reference to index queue
            CloudQueueClient client = cloudStorageAccount.CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference("searchindexqueue");
            queue.CreateIfNotExist();
            queue.Clear();

            // Create lucene index writer and optimize index (only on startup)
            _Directory = new AzureDirectory(cloudStorageAccount, "LuceneStorage", new RAMDirectory());

            while (true)
            {
                try
                {
                    int queueItems = queue.RetrieveApproximateMessageCount();
                    Trace.WriteLine(DateTime.Now.ToString() + " [QUEUE_CHECK] " + queueItems + " items.");
                    if (queueItems > 0)
                    {
                        // Get index writer
                        if (IndexWriter.IsLocked(_Directory)) _Directory.ClearLock("write.lock");
                        IndexWriter indexWriter = new IndexWriter(_Directory, new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29), false, new IndexWriter.MaxFieldLength(1000));
                        indexWriter.SetRAMBufferSizeMB(10.0);
                        indexWriter.SetUseCompoundFile(false);
                        indexWriter.SetMaxMergeDocs(10000);
                        indexWriter.SetMergeFactor(100);

                        try
                        {
                            // Get dictionary to skip duplicated queue items
                            Dictionary<long, DateTime> updatedSummaries = new Dictionary<long, DateTime>();

                            // Retrieve batch of messages to iterate
                            var msgs = queue.GetMessages(32);
                            while (msgs.Count() > 0)
                            {
                                foreach (var msg in msgs)
                                {
                                    Trace.WriteLine(DateTime.Now.ToString() + " [MSG] " + msg.AsString);
                                    long summaryId = long.Parse(msg.AsString);

                                    // zero = re-index the search index
                                    // positive number = add/update PointDataSummary to index
                                    // negative number = delete PointDataSummary from index
                                    if (summaryId == 0)
                                    {
                                        Trace.WriteLine(DateTime.Now.ToString() + " [REINDEX]");
                                        indexWriter.Close();
                                        indexWriter = new IndexWriter(_Directory, new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29), true, new IndexWriter.MaxFieldLength(1000));
                                        indexWriter.SetRAMBufferSizeMB(10.0);
                                        indexWriter.SetUseCompoundFile(false);
                                        indexWriter.SetMaxMergeDocs(10000);
                                        indexWriter.SetMergeFactor(100);
                                        List<PointDataSummary> summaries = PointDataSummary.All().ToList();
                                        updatedSummaries = new Dictionary<long, DateTime>();
                                        foreach (var summary in summaries)
                                        {
                                            Index(indexWriter, summary);
                                            Trace.WriteLine(DateTime.Now.ToString() + " [INDEX] " + summary.Id);
                                            updatedSummaries.Add(summaryId, DateTime.UtcNow);
                                        }
                                    }
                                    else if (updatedSummaries.ContainsKey(summaryId) && ((DateTime)updatedSummaries[summaryId]) >= msg.InsertionTime)
                                    {
                                        Trace.WriteLine(DateTime.Now.ToString() + " [SKIPPED] " + msg.AsString);
                                    }
                                    else
                                    {
                                        if (summaryId < 0)
                                        {
                                            indexWriter.DeleteDocuments(new Lucene.Net.Index.Term("point_id", (-summaryId).ToString()));
                                        }
                                        else
                                        {
                                            var summary = PointDataSummary.SingleOrDefault(p => p.Id == summaryId);
                                            if (summary != null)
                                            {
                                                Index(indexWriter, summary);
                                                Trace.WriteLine(DateTime.Now.ToString() + " [INDEX] " + summary.Id);
                                            }
                                        }
                                        updatedSummaries.Remove(summaryId);
                                        updatedSummaries.Add(summaryId, DateTime.UtcNow);
                                    }
                                    // Delete message from queue
                                    queue.DeleteMessage(msg);
                                }
                                // Retrieve batch of messages to iterate
                                msgs = queue.GetMessages(32);
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.ToString());
                        }
                        finally
                        {
                            indexWriter.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now + " [ERROR] " + ex.Message);
                }
                finally
                {
                    Trace.WriteLine(DateTime.Now.ToString() + " [QUEUE_CHECK] End");
                    Thread.Sleep(_SleepInMinutes * 60 * 1000);
                }
            }
        }

        /// <summary>
        /// Adds the PointDataSummary to the lucene index specified by the IndexWriter
        /// </summary>
        /// <param name="indexWriter"></param>
        /// <param name="summary"></param>
        public void Index(IndexWriter indexWriter, PointDataSummary summary)
        {
            // Delete the current document if it exists already
            indexWriter.DeleteDocuments(new Lucene.Net.Index.Term("point_id", summary.Id.ToString()));

            // Create Lucene document and add the indexed fields.
            Document doc = new Document();
            doc.Add(new Field("point_id", summary.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO));
            doc.Add(new Field("Name", summary.Name, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.NO));
            doc.Add(new Field("Tags", summary.Tag, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.NO));
            doc.Add(new Field("Description", summary.Description, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.NO));
            doc.Add(new Field("LayerId", summary.LayerId, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.NO));
            indexWriter.AddDocument(doc);
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;
            RoleEnvironment.Changing += RoleEnvironmentChanging;
            return base.OnStart();
        }

        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            if (e.Changes.Any(change => change is RoleEnvironmentConfigurationSettingChange))
            {
                e.Cancel = true;
            }
        }
    }
}
