using Microsoft.AspNetCore.Http;
using Project2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Project2.Repository
{
    public class BlobStorageRepo : IBlobStorageRepo
    {
        private StorageCredentials _storageCredentials;
        private CloudStorageAccount _cloudStorageAccount;
        private CloudBlobClient _cloudBlobClient;
        private CloudBlobContainer _cloudBlobContainer;
        private string containerName = "photos";
        private string downloadPath = @"F:\Uni 2021\Semester 2\CMPG323\Project 2\Final version";

        public BlobStorageRepo()
        {
            string accountName = "project2photosharing";
            string key = "j2vtNig7kUeIdXjjrxPpcrkplaCFm4Ano6ad45bRKr+xqkpD1bQYck3k9iMqaJyHkWUo2a72LIAqT92OpdsVHw==";
            _storageCredentials = new StorageCredentials(accountName, key);
            _cloudStorageAccount = new CloudStorageAccount(_storageCredentials, true);
            _cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();
            _cloudBlobContainer = _cloudBlobClient.GetContainerReference(containerName);
        }

        public bool DeleteBlob(string file, string fileExtension)
        {
            _cloudBlobContainer = _cloudBlobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(file + "." + fileExtension);
            bool deleted = blockBlob.DeleteIfExists();

            return deleted;
        }

        public async Task<bool> DownloadBlob(string file, string fileExtension)
        {
            _cloudBlobContainer = _cloudBlobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(file + "." + fileExtension);

            using (var fileStream = File.OpenWrite(downloadPath + file + "." + fileExtension))
            {
                await blockBlob.DownloadToStreamAsync(fileStream);
                return true;
            }
        }

        public IEnumerable<BlobViewModel> GetBlobs()
        {
            var context = _cloudBlobContainer.ListBlobs().ToList();
            IEnumerable<BlobViewModel> VM = context.Select(x => new BlobViewModel
            {
                BlobContainerName = x.Container.Name,
                StorageUri = x.StorageUri.PrimaryUri.ToString(),
                PrimaryUri = x.StorageUri.PrimaryUri.ToString(),
                ActualFileName = x.Uri.AbsoluteUri.Substring(x.Uri.AbsoluteUri.LastIndexOf("/" + 1)),
                fileExtension = Path.GetExtension(x.Uri.AbsoluteUri.Substring(x.Uri.AbsoluteUri.LastIndexOf("/") + 1))
            }).ToList();

            return VM;
        }

        //public bool Uploadfile(IFormFile blobFile)
        //{
        //    if (blobFile == null)
        //        return false;

        //    _cloudBlobContainer = _cloudBlobClient.GetContainerReference(containerName);
        //    CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(blobFile.FileName);

        //    using (var fileStream = (blobFile.InputStream))
        //    {
        //        blockBlob.UploadFromStream(fileStream);
        //    }

        //    return true;
        //}

        public bool Upload (IFormFile blobFile)
        {
            if (blobFile == null)
                return false;

            _cloudBlobContainer = _cloudBlobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(blobFile.FileName);

            byte[] buffer = new byte[blobFile.Length];
            var resultInBytes = ConvertToBytes(blobFile);
            Array.Copy(resultInBytes, buffer, resultInBytes.Length);
            return true;
        }

        private byte[] ConvertToBytes(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.OpenReadStream().CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
