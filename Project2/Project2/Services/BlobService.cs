using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Project2.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Project2.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("photos");
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public async Task<Project2.Models.BlobInfo> GetBlobAsync(string name)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("photos");
                var blobClient = containerClient.GetBlobClient(name);
                var blobDownloadInfo = await blobClient.DownloadAsync();
                return new Project2.Models.BlobInfo(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public async Task<IEnumerable<string>> ListBlobAsync()
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("photos");
                var items = new List<string>();

                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    items.Add(blobItem.Name);
                }

                return items;
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public async Task UploadContentBlobAsync(string content, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("photos");
                var blobClient = containerClient.GetBlobClient(fileName);
                var bytes = Encoding.UTF8.GetBytes(content);
                await using var memoryStream = new MemoryStream(bytes);
                await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = fileName.GetContentType() });
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public async Task UploadFileBlobAsync(string filePath, string fileName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient("photos");
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(filePath, new BlobHttpHeaders { ContentType = filePath.GetContentType() });
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public async Task DownloadFile(string fileName)
        {
            try
            {
                string downloadPath = "C:\\Users\\shene\\Downloads\\";
                var containerClient = _blobServiceClient.GetBlobContainerClient("photos");
                var blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.DownloadToAsync(downloadPath);
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }
    }
}