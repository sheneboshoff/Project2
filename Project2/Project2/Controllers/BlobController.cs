using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Project2.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Mvc;

namespace Project2.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlobStorageRepo repo;
        public BlobController(IBlobStorageRepo _repo)
        {
            repo = _repo;
        }

        public ActionResult Index()
        {
            var blobVM = repo.GetBlobs();
            return View(blobVM);
        }

        public JsonResult RemoveBlob(string file, string extension)
        {
            bool isDeleted = repo.DeleteBlob(file, extension);
            return Json(isDeleted/*, JsonRequestBehavior.AllowGet*/);
        }

        public async Task<ActionResult> DownloadBlob(string file, string extension)
        {
            bool isDownload = await repo.DownloadBlob(file, extension);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult UploadBlob()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadBlob(IFormFile uploadFileName)
        {
            bool isUploaded = repo.UploadFile(uploadFileName);
            if (isUploaded)
            {
                return RedirectToAction("Index");
            }
            return View(); 
        }
    }
}
