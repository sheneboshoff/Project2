using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Controllers
{
    public class CookiesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult WriteCookie(string name, string value)
        {
            Response.Cookies.Append(name, value);
            return View();
        }

        public IActionResult ReadCookie(string name)
        {
            string cookie = Request.Cookies[name];
            return View();
        }

        public bool writeCookie(string name, string value)
        {
            Response.Cookies.Append(name, value);
            return true;
        }

        public string readCookie(string name)
        {
            return Request.Cookies[name].ToString();
        }
    }
}
