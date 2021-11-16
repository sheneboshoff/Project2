using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Project2.Models;
using Project2.Services;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Project2.Controllers
{
    public class PhotoViewController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IBlobService _blobService;

        public PhotoViewController(IConfiguration configuration, IBlobService blobService)
        {
            _configuration = configuration;
            _blobService = blobService;
        }

        // GET: PhotoView
        public IActionResult Index(string searchString)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand("ViewAllPhotos", sqlConnection);
                SqlDataAdapter adap = new SqlDataAdapter("ViewAllPhotos", sqlConnection);
                adap.SelectCommand.CommandType = CommandType.StoredProcedure;
                adap.SelectCommand.Parameters.AddWithValue("UserId", getUserId());
                adap.Fill(dt);
            }
            return View(dt);

            if (!String.IsNullOrEmpty(searchString))
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("ViewAllPhotos", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("UserId", getUser());
                    SqlDataAdapter adap = new SqlDataAdapter("ViewAllPhotos", sqlConnection);
                    adap.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adap.Fill(dt);
                }
            }
        }

        // GET: PhotoView/Edit/5
        public IActionResult AddOrEdit(int? id)
        {
            PhotoViewModel photoViewModel = new PhotoViewModel();
            if (id > 0)
            {
                photoViewModel = FetchPhotoById(id);
            }
            return View(photoViewModel);
        }

        // POST: PhotoView/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit([Bind("PhotoId,Photo_Geolocation,Photo_Tags,Photo_CaptureDate")] PhotoViewModel photoViewModel)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("PhotoAddOrEdit", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("PhotoId", getNextVal());
                    cmd.Parameters.AddWithValue("Photo_Geolocation", photoViewModel.Photo_Geolocation);
                    if (photoViewModel.Photo_Tags == null)
                    {
                        cmd.Parameters.AddWithValue("Photo_Tags", photoViewModel.Photo_Tags);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("Photo_Tags", photoViewModel.Photo_Tags + ",");
                    }
                    cmd.Parameters.AddWithValue("Photo_CaptureDate", photoViewModel.Photo_CaptureDate);
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(photoViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("UserId,Photo_Geolocation,Photo_Tags,Photo_CaptureDate")] PhotoViewModel photoViewModel)
        {
            if (ModelState.IsValid)
            {
                string fileName = HttpContext.Session.GetString("fileName");
                string fileExtension = "";
                string[] file = fileName.Split('.');
                fileName = file[0];
                fileExtension = file[1];

                if (!validateExtension(fileExtension))
                {
                    return View("ExtError");
                }
                else
                {
                    using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                    {
                        sqlConnection.Open();
                        SqlCommand cmd = new SqlCommand("PhotoAddOrEdit", sqlConnection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("PhotoId", getNextVal());
                        cmd.Parameters.AddWithValue("UserId", getUserId());
                        cmd.Parameters.AddWithValue("Photo_Name", fileName);
                        cmd.Parameters.AddWithValue("Photo_Format", fileExtension);
                        cmd.Parameters.AddWithValue("Photo_Geolocation", photoViewModel.Photo_Geolocation);
                        if (photoViewModel.Photo_Tags == null)
                        {
                            cmd.Parameters.AddWithValue("Photo_Tags", photoViewModel.Photo_Tags);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("Photo_Tags", photoViewModel.Photo_Tags + ",");
                        }
                        cmd.Parameters.AddWithValue("Photo_CaptureDate", photoViewModel.Photo_CaptureDate);
                        cmd.ExecuteNonQuery();
                        fileName = "";
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(photoViewModel);
        }

        public IActionResult ShareWith([Bind("PhotoId,Creator")] User userModel)
        {
            return RedirectToAction("Index", "User");
            //DataTable dt = new DataTable();
            //using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
            //{
            //    sqlConnection.Open();
            //    SqlCommand cmd = new SqlCommand("ViewAllPhotos", sqlConnection);
            //    SqlDataAdapter adap = new SqlDataAdapter("ViewAllUsers", sqlConnection);
            //    adap.SelectCommand.CommandType = CommandType.StoredProcedure;
            //    adap.SelectCommand.Parameters.AddWithValue("UserId", getUserId());
            //    adap.Fill(dt);
            //}
            //return View(dt);

            //using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
            //{
            //    sqlConnection.Open();
            //    SqlCommand cmd = new SqlCommand("SharePhotoWithUser", sqlConnection);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    cmd.Parameters.AddWithValue("PhotoId", photoViewModel.PhotoId);
            //    cmd.Parameters.AddWithValue("UserId", photoViewModel.UserId);
            //    RedirectToPage("User");
            //    cmd.Parameters.AddWithValue("SharedWith", id);
            //    cmd.ExecuteNonQuery();
            //}
            //return RedirectToAction(nameof(Index));
        }

        // GET: PhotoView/Delete/5
        public IActionResult Delete(int? id)
        {
            PhotoViewModel photoViewModel = new PhotoViewModel();
            return View();
        }

        // POST: PhotoView/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand("PhotoDeleteById", sqlConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PhotoId", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction(nameof(Index));
        }

        private string getUser()
        {
            return HttpContext.User.Identity.Name;
        }

        private string getUserId()
        {
            string result;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetUserIdByName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserName", getUser());
                result = cmd.ExecuteScalar().ToString();
            }
            return result;
        }

        private int getNextVal()
        {
            int result;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetNextSeqValue", con);
                cmd.CommandType = CommandType.StoredProcedure;
                result = (int)cmd.ExecuteScalar();
            }
            return result;
        }

        [NonAction]
        public PhotoViewModel FetchPhotoById(int? id)
        {
            PhotoViewModel photoViewModel = new PhotoViewModel();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
            {
                DataTable dt = new DataTable();
                con.Open();
                SqlDataAdapter adap = new SqlDataAdapter("GetPhotoById", con);
                adap.SelectCommand.CommandType = CommandType.StoredProcedure;
                adap.SelectCommand.Parameters.AddWithValue("PhotoId", id);
                adap.Fill(dt);

                if (dt.Rows.Count == 1)
                {
                    photoViewModel.PhotoId = Convert.ToInt32(dt.Rows[0]["PhotoId"].ToString());
                    photoViewModel.UserId = dt.Rows[1]["UserId"].ToString();
                    photoViewModel.Photo_Name = dt.Rows[2]["Photo_Name"].ToString();
                    photoViewModel.Photo_Format = dt.Rows[3]["Photo_Format"].ToString();
                    photoViewModel.Photo_Geolocation = dt.Rows[4]["Photo_Geolocation"].ToString();
                    photoViewModel.Photo_Tags = dt.Rows[5]["Photo_Tags"].ToString();
                    photoViewModel.Photo_CaptureDate = dt.Rows[6]["Photo_CaptureDate"].ToString();
                }

                return photoViewModel;
            }
        }

        public string getChosenUser()
        {
            string result;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("GetUserIdByName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserName", getUser());
                result = cmd.ToString();
            }
            return result;
        }

        public IActionResult UploadFile()
        {
            return View("UploadBlob");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(UploadFileRequest request)
        {
            HttpContext.Session.SetString("fileName", request.FileName);
            await _blobService.UploadFileBlobAsync(request.FilePath, request.FileName);
            return View("AddOrEdit");
        }

        public bool validateExtension(string ext)
        {
            if (ext.ToLower() != "jpg" | ext.ToLower() != "jpeg" | ext.ToLower() != "png" | ext.ToLower() != "bmp" | ext.ToLower() != "gif" | ext.ToLower() != "ico" | ext.ToLower() != "tiff")
            {
                return true;
            }
            else
                return false;
        }
    }
}