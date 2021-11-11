using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project2.Data;
using Project2.Models;
using System.Data;

namespace Project2.Controllers
{
    public class PhotoViewController : Controller
    {
        private readonly IConfiguration _configuration;

        public PhotoViewController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: PhotoView
        public IActionResult Index()
        {
            return View();
        }     

        // GET: PhotoView/Edit/5
        public IActionResult Edit(int? id)
        {
            PhotoViewModel photoViewModel = new PhotoViewModel();
            return View(photoViewModel);
        }

        // POST: PhotoView/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("PhotoId,UserId,Photo_Name,Photo_Format,Photo_Geolocation,Photo_Tags,Photo_CaptureDate")] PhotoViewModel photoViewModel)
        {
            if (ModelState.IsValid)
            {
                using(SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("PhotoDBCon")))
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("PhotoAddOrEdit", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("PhotoId", photoViewModel.PhotoId);
                    cmd.Parameters.AddWithValue("UserId", );
                }
                return RedirectToAction(nameof(Index));
            }
            return View(photoViewModel);
        }

        // GET: PhotoView/Delete/5
        public IActionResult Delete(int? id)
        {
            

            return View();
        }

        // POST: PhotoView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
           
            return RedirectToAction(nameof(Index));
        }

        private string getUser()
        {
            return HttpContext.User.Identity.Name;
        }
    }
}
