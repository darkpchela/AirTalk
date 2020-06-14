using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AirTalk.Models.DBModels;
using AirTalk.Models.ViewModels;
using AirTalk.Services.CommandTranslator;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using AirTalk.Services;
using System.Text.Json;

namespace AirTalk.Controllers
{
    //[Authorize]
    public class MainController : Controller
    {
        private readonly MainDbContext db;
        private readonly ILogger<MainController> logger;
        private MainInfoViewModel mainVM;
        private TerminalResultBuilder resultBuilder;

        public MainController(MainDbContext db, ILogger<MainController> logger, cmdTranslator cmdTranslator, TerminalResultBuilder resultBuilder)
        {
            this.db = db;
            this.logger = logger;
            this.resultBuilder = resultBuilder;
        }

        [HttpPost]
        public IActionResult GetSessionInfo()
        {
            var session = HttpContext.Session.SessionInfo();
            if (HttpContext.Session.Keys.Contains("themes"))
            {
                int[] themeList = HttpContext.Session.Get<List<Message>>("themes").Select(t => t.id).ToArray();
                var messages = (from m in db.messages
                                join t in db.themes on m.themeId equals t.id
                                join u in db.users on m.userSenderId equals u.id
                                where themeList.Contains(t.id)
                                orderby m.time
                                select new { userSender = u.login, m.id, m.themeId, m.time, m.text }).ToArray();
                session.Add("messages", JsonSerializer.Serialize(messages));
            }
            return Json(session);
        }

        //[Route("Main/Index/{id?}")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Account()
        {
            return View();
        }

        public IActionResult SelectTheme(int? id)
        {
            if (id!=null&&db.themes.First(t=>t.id==id)!=null)
            {
                HttpContext.Session.SetInt32("currentThemeId", id.Value);
                return RedirectToAction("Index");
            }
            else
            {
                HttpContext.Session.SetInt32("currentThemeId", -1);
                mainVM = new MainInfoViewModel(this.db);
                return View(mainVM);
            }
            
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
                Theme newTheme = themeViewModel.GetDBModel(db.users.First(u=>u.login==User.Identity.Name).id);
                db.themes.Add(newTheme);
                db.SaveChanges();
                int themeId = newTheme.id;

                RedirectToAction("SelectTheme", themeId);

            }
            return View();
        }
    }
}