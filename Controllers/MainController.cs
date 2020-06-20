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
    public class MainController : Controller
    {
        private readonly MainDbContext db;

        public MainController(MainDbContext db, ILogger<MainController> logger)
        {
            this.db = db;
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

        public IActionResult Index()
        {
            return View();
        }
    }
}