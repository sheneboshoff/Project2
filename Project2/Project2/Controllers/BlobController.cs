using Microsoft.AspNetCore.Mvc;
using Project2.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlobStorageRepo repo;
        public BlobController(IBlobStorageRepo _repo)
        {
            repo = _repo;
        }

        public IActionResult Index()
        {
            var blobVM = repo.GetBlobs();
            return View(blobVM);
        }
    }
}
