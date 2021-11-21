using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace Project2.Controllers
{
    public class UserController : Controller
    {
        private IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            try
            {
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

        public IActionResult Select(string id)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("PhotoDBCon")))
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("GetUserById", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("UserId", id);
                    cmd.ExecuteNonQuery();
                }
                return RedirectToPage("PhotoView/ShareWith/id");
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong", e);
            }
        }
    }
}