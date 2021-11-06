using Microsoft.AspNetCore.Http;
using Project2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Project2.Repository
{
    interface IBlobStorageRepo
    {
        IEnumerable<BlobViewModel> GetBlobs();
        bool DeleteBlob(string file, string fileExtension);
        bool UploadFile(IFormFile blobFile);
        Task<bool> DownloadBlob(string file, string fileExtension);
    }
}
