using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AirTalk.Models.DBModels;
using AirTalk.Models.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;

namespace AirTalk.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        private readonly MainDbContext db;
        private readonly ILogger<MainController> logger;
        private MainInfoViewModel mainVM;

        public MainController(MainDbContext db, ILogger<MainController> logger)
        {
            this.db = db;
            this.logger = logger;
        }

    
        [Route("Main/Index/{id?}")]
        public IActionResult Index(int? id)
        {
            mainVM = new MainInfoViewModel(this.db);
            if (id!=null&&id>0&&db.themes.Any(t=>t.id==id))
            {
                HttpContext.Session.SetInt32("currentThemeId", id.Value);
                mainVM.currentTheme = db.themes.FirstOrDefault(t => t.id == HttpContext.Session.GetInt32("currentThemeId"));
            }
            else
            {
                HttpContext.Session.SetInt32("currentThemeId", -1);
            }
            mainVM.currentUser = this.db.users.FirstOrDefault(u => u.login == User.Identity.Name);

            return View(mainVM);
        }
        public IActionResult Account()
        {
            return View();
        }

        public IActionResult SelectTheme(int? id)
        {
            if (id==null)
            {
                mainVM = new MainInfoViewModel(this.db);
                return View(mainVM);
            }
            else
            if (db.themes.FirstOrDefault(t=>t.id==id)!=null)
            {
                HttpContext.Session.SetInt32("currentThemeId", id.Value);
                return Redirect("~/Main/Index/" + id);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CreateTheme()
        {

            return View();
        }
        [HttpPost]
        public IActionResult CreateTheme(CreateThemeViewModel themeViewModel)
        {
            if (db.themes.Any(t=>t.name==themeViewModel.name))
            {
                ModelState.AddModelError(nameof(themeViewModel.name), "Theme with the same name already exists");
            }
            if (ModelState.IsValid)
            {
                Theme newTheme = themeViewModel.GetDBModel(db.users.FirstOrDefault(u=>u.login==User.Identity.Name).id);
                db.themes.Add(newTheme);
                db.SaveChanges();
                int themeId = newTheme.id;

                RedirectToAction("SelectTheme", themeId);

            }
            return View();
        }

        public IActionResult ChatTest()
        {
            return View();
        }
    }
}