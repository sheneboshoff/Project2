using Microsoft.AspNetCore.Mvc;
using Project2.Models;
using Project2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Controllers
{
    [Route("blobs")]
    public class BlobExplorerController : Controller
    {
        private readonly IBlobService _blobService;

        public BlobExplorerController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListBlobs()
        {
            return Ok(await _blobService.ListBlobAsync());
        }

        [HttpGet("{blobName}")]
        public async Task<IActionResult> GetBlob(string blobName)
        {
            var data = await _blobService.GetBlobAsync(blobName);
            return File(data.Content, data.ContentType);
        }

        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadFile(UploadFileRequest request)
        {
            await _blobService.UploadFileBlobAsync(request.FilePath, request.FileName);
            return Ok();
        }

        [HttpPost("uploadContent")]
        public async Task<IActionResult> UploadContent([FromBody] UploadContentRequest request)
        {
            await _blobService.UploadContentBlobAsync(request.Content, request.FileName);
            return Ok();
        }

        [HttpDelete("{blobName}")]
        public async Task<IActionResult> DeleteFile(string blobName)
        {
            await _blobService.DeleteBlobAsync(blobName);
            return Ok();
        }
    }
}
