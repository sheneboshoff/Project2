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
        private int _photoId;

        public PhotoViewController(IConfiguration configuration, IBlobService blobService)
        {
            _configuration = configuration;
            _blobService = blobService;
        }

        // GET: PhotoView
        public IActionResult Index(/*string searchString*/)
        {
            try
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
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
            //    {
            //        sqlConnection.Open();
            //        SqlCommand cmd = new SqlCommand("ViewAllPhotos", sqlConnection);
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.Parameters.AddWithValue("UserId", getUser());
            //        SqlDataAdapter adap = new SqlDataAdapter("ViewAllPhotos", sqlConnection);
            //        adap.SelectCommand.CommandType = CommandType.StoredProcedure;
            //        adap.Fill(dt);
            //    }
            //}
        }

        // GET: PhotoView/Edit/5
        public IActionResult Edit(int? id)
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
        public IActionResult Edit([Bind("PhotoId,Photo_Geolocation,Photo_Tags,Photo_CaptureDate")] PhotoViewModel photoViewModel)
        {
            try
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
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("UserId,Photo_Geolocation,Photo_Tags,Photo_CaptureDate")] PhotoViewModel photoViewModel)
        {
            try
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
                            SqlCommand cmd = new SqlCommand("CreateNewPhoto", sqlConnection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("PhotoId", getNextVal());
                            cmd.Parameters.AddWithValue("UserId", getUserId());
                            cmd.Parameters.AddWithValue("Photo_Name", fileName);
                            cmd.Parameters.AddWithValue("Photo_Format", fileExtension);
                            if (photoViewModel.Photo_Geolocation == null)
                            { 
                                cmd.Parameters.AddWithValue("Photo_Geolocation", null); 
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("Photo_Geolocation", photoViewModel.Photo_Geolocation);
                            }
                            if (photoViewModel.Photo_Tags == null)
                            {
                                cmd.Parameters.AddWithValue("Photo_Tags", null);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("Photo_Tags", photoViewModel.Photo_Tags + ",");
                            }
                            if (photoViewModel.Photo_CaptureDate == null)
                            {
                                cmd.Parameters.AddWithValue("Photo_CaptureDate", null);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("Photo_CaptureDate", photoViewModel.Photo_CaptureDate); 
                            }
                            
                            cmd.ExecuteNonQuery();
                            fileName = "";
                        }
                        return RedirectToAction(nameof(Index));
                    }
                }
                return View(photoViewModel);
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public IActionResult ShareWith(int photoId)
        {
            try
            {
                _photoId = photoId;
                DataTable dt = new DataTable();
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                {
                    sqlConnection.Open();
                    SqlDataAdapter adap = new SqlDataAdapter("ViewAllUsers", sqlConnection);
                    adap.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adap.Fill(dt);
                }
                return View(dt);
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        [Route("ShareWith/{id:int}")]
        public IActionResult ShareWith(int? photoId)
        {
            UserPhoto userPhoto = new UserPhoto();
            return View();
        }

        public IActionResult Select(string email)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("SharePhotoWithUser", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("PhotoId", _photoId);
                    cmd.Parameters.AddWithValue("Creator", getUserId());
                    cmd.Parameters.AddWithValue("SharedWith", getUserByEmail(email));
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
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
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("PhotoDeleteById", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("PhotoId", id);
                    cmd.ExecuteNonQuery();
                }
                _ = DeleteFile(getPhotoNameById(id));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        private string getUser()
        {
            return HttpContext.User.Identity.Name;
        }

        private string getUserId()
        {
            try
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
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        private string getUserByEmail(string email)
        {
            try
            {
                string result;
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("GetUserByEmail", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Email", email);
                    result = cmd.ExecuteScalar().ToString();
                }
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        private int getNextVal()
        {
            try
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
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        [NonAction]
        public PhotoViewModel FetchPhotoById(int? id)
        {
            try
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
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public string getChosenUser()
        {
            try
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
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public IActionResult UploadFile()
        {
            return View("UploadBlob");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(UploadFileRequest request)
        {
            try
            {
                HttpContext.Session.SetString("fileName", request.FileName);
                await _blobService.UploadFileBlobAsync(request.FilePath, request.FileName);
                return View("Create");
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public async Task<IActionResult> DeleteFile(string filename)
        {
            try
            {
                await _blobService.DeleteBlobAsync(filename);
                return View("Index");
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            try
            {
                await _blobService.DownloadFile(filename);
                return View("Index");
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public bool validateExtension(string ext)
        {
            try
            {
                if (ext.ToLower() != "jpg" | ext.ToLower() != "jpeg" | ext.ToLower() != "png" | ext.ToLower() != "bmp" | ext.ToLower() != "gif" | ext.ToLower() != "ico" | ext.ToLower() != "tiff")
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public IActionResult RemoveAccess(int photoId)
        {
            try
            {
                _photoId = photoId;
                DataTable dt = new DataTable();
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                {
                    sqlConnection.Open();
                    SqlDataAdapter adap = new SqlDataAdapter("UsersSharedWith", sqlConnection);
                    adap.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adap.SelectCommand.Parameters.AddWithValue("Creator", getUserId());
                    adap.Fill(dt);
                }
                return View(dt);
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public IActionResult SharedWithMe()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                {
                    sqlConnection.Open();
                    SqlDataAdapter adap = new SqlDataAdapter("SharedWithUser", sqlConnection);
                    adap.SelectCommand.CommandType = CommandType.StoredProcedure;
                    adap.SelectCommand.Parameters.AddWithValue("SharedWith", getUserId());
                    adap.Fill(dt);
                }
                return View(dt);
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        public IActionResult SelectToRemove(string email)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("RemovePhotoAccess", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("PhotoId", _photoId);
                    cmd.Parameters.AddWithValue("Creator", getUserId());
                    cmd.Parameters.AddWithValue("SharedWith", getUserByEmail(email));
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }

        //public IActionResult SharedWithMe()
        //{
        //    try
        //    {
        //        using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
        //        {
        //            sqlConnection.Open();
        //            SqlCommand cmd = new SqlCommand("UsersSharedWith", sqlConnection);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("SharedWith", getUserId());
        //            cmd.ExecuteNonQuery();
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("Something went wrong", e);
        //    }
        //}

        public string getPhotoNameById(int id)
        {
            try
            {
                string result;
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Project2DbContextConnection")))
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("GetPhotoNameById", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("PhotoId", id);
                    result = cmd.ExecuteScalar().ToString();
                }
                return result;
            }
            catch (Exception e)
            {

                throw new Exception("Something went wrong", e);
            }
        }
    }
}