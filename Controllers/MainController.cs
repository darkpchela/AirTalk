using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AirTalk.Models.InsideModels;
using AirTalk.Models.ViewModels;

namespace AirTalk.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        private readonly MainDbContext db;
        private MainIndexViewModel mainVM;
        public MainController(MainDbContext db)
        {
            this.db = db;
        }
        [HttpGet]
        public IActionResult Index()
        {
            mainVM = new MainIndexViewModel(this.db);
            mainVM.currentUser = this.db.users.FirstOrDefault(u => u.login == User.Identity.Name);
            return View(mainVM);
        }
        public IActionResult Account()
        {
            return View();
        }

        public IActionResult SelectTheme(int id)
        {
            mainVM = new MainIndexViewModel(this.db);
            mainVM.currentUser = this.db.users.FirstOrDefault(u => u.login == User.Identity.Name);
            mainVM.currentTheme = db.themes.FirstOrDefault(t => t.id == id);
            return View("Index", mainVM);
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

            }
            return View(themeViewModel);
        }
    }
}