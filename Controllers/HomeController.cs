using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AirTalk.Models;
using AirTalk.Models.ViewModels;
using AirTalk.Models.InsideModels;
using AirTalk.Services;
using Microsoft.AspNetCore.Http;

namespace AirTalk.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MainDbContext db;
        
        public HomeController(ILogger<HomeController> logger, MainDbContext dbContext, UserCounterService userCounter)
        {
            _logger = logger;
            db = dbContext;
        }

        public IActionResult Index()
        {
            RedirectToAction("Index", "Main");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(UserRegistrationVM user)
        {

            if (ModelState.IsValid)
            {
                db.Add(user.GetDbModel());
                db.SaveChanges();
                return Content("Registration completed!");
            }
            else
                return View(user);

        }
        [AcceptVerbs("Get","Post")]
        public IActionResult CheckLogin(string login)
        {
            _logger.LogInformation("checking");
            var checker = db.users.FirstOrDefault(u=>u.login==login);
            if (checker==null)
                return Json(true);
            else
                return Json(false);
        }
        public IActionResult CheckEmail(string email)
        {
            var checker = db.users.FirstOrDefault(u => u.email == email);
            if (checker == null)
                return Json(true);
            else
                return Json(false);
        }
        //[HttpPost]
        //public IActionResult Login(UserSignInVM user)
        //{
        //    var checker = db.users.FirstOrDefault(u => u.login == user.loginOrEmail || u.email == user.loginOrEmail);
        //    if (checker==null)
        //    {
        //        ModelState.AddModelError(nameof(user.loginOrEmail), "Account is not exists");
        //    }
        //    else
        //    if(user.password!=checker.password)
        //    {
        //        ModelState.AddModelError(nameof(user.password), "Incorrect password");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        HttpContext.Session.SetString(sessionKey, checker.id.ToString());
        //        return RedirectToAction("Index", "Main");
        //    }
        //    else
        //        return View("Index");
        //}
    }
}
