//    License: Microsoft Public License (Ms-PL) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net;
using Lucene.Net.Store;
using Microsoft.WindowsAzure.StorageClient;
using System.Diagnostics;

namespace Lucene.Net.Store.Azure
{
    public class AzureLock : Lock
    {
        private string _lockFile;
        private AzureDirectory _azureDirectory;

        public AzureLock(string lockFile, AzureDirectory directory)
        {
            _lockFile = lockFile;
            _azureDirectory = directory;
        }

        #region Lock methods
        override public bool IsLocked()
        {
            var blob = _azureDirectory.BlobContainer.GetBlobReference(_lockFile);
            try
            {
                blob.FetchAttributes();
                return true;
            }
            catch(StorageClientException err)
            {
                if (err.ErrorCode != StorageErrorCode.ResourceNotFound)
                    Trace.TraceError(err.ToString());
                return false;
            }
        }

        public override bool Obtain()
        {
            try
            {
                if (IsLocked())
                    return false;

                var blob = _azureDirectory.BlobContainer.GetBlobReference(_lockFile);
                blob.UploadText("lock");
                Debug.WriteLine(string.Format("Obtained lock {0}", _lockFile));
                return true;
            }
            catch (StorageClientException err)
            {
                switch (err.ErrorCode)
                {
                    case StorageErrorCode.ContainerNotFound:
                        // container is missing, we should create it.
                        _azureDirectory.BlobContainer.Delete();
                        _azureDirectory.CreateContainer();
                        return this.Obtain();
                }
                return false;
            }
        }

        public override void Release()
        {
            Debug.WriteLine(string.Format("Releasing lock {0}", _lockFile));
            var blob = _azureDirectory.BlobContainer.GetBlobReference(_lockFile);
            blob.DeleteIfExists();
        }
        #endregion

        public override System.String ToString()
        {
            return "Lock@" + _lockFile;
        }
    }
}
