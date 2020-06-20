using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using AirTalk.Services.CommandTranslator;
using AirTalk.Services;
using Microsoft.Extensions.Logging;
using AirTalk.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using AirTalk.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace AirTalk.Controllers
{
    public class TerminalController : Controller
    {
        MainDbContext db;
        cmdTranslator cmdTranslator;
        ILogger<TerminalController> logger;
        TerminalResultBuilder terminalResultBuilder;
        public TerminalController(cmdTranslator cmdTranslator, ILogger<TerminalController> logger,
            TerminalResultBuilder terminalResultBuilder, MainDbContext db)
        {
            this.db = db;
            this.cmdTranslator = cmdTranslator;
            this.logger = logger;
            this.terminalResultBuilder = terminalResultBuilder;
        }

        [HttpPost]
        public IActionResult InitializeCommand(string request)
        {
            var cmdResponse = cmdTranslator.ReadCommand(request);
            terminalResultBuilder.AddAjaxFunc(cmdResponse.action, cmdResponse.cmdParams);
            var result = terminalResultBuilder.Build();
            return Json(result);
        }
        
        [HttpPost]
        public IActionResult error(string mes)
        {
            terminalResultBuilder.AddJSFuncInline("addTextToConsole",  mes);
            return Json(terminalResultBuilder.Build());
        }
        
        [HttpPost]
        public IActionResult login()
        {
            terminalResultBuilder.AddAspView(this, "SignIn");
            return Json(terminalResultBuilder.Build());
        }

        [HttpPost]
        public IActionResult logout()
        {
            terminalResultBuilder.AddAjaxFunc("Account/Logout");
            return Json(terminalResultBuilder.Build());
        }
        [HttpPost]
        public IActionResult registration()
        {
            terminalResultBuilder.AddAspView(this, "Registration");
            return Json(terminalResultBuilder.Build());
        }
        [HttpPost]
        public IActionResult select(int? themeId)
        {
            if (themeId==null)
            {
                var themes = db.themes.OrderBy(t => t.creationTime).Take(10);
                foreach(var t in themes)
                {
                    terminalResultBuilder.AddJSFuncInline("addTextToConsole", t.name + ", id: " + t.id.ToString());
                }
                var result = terminalResultBuilder.Build();
                return Json(result);
            }

            if (db.themes.Any(t=>t.id==themeId))
            {
                var themeModel = db.themes.First(t => t.id == themeId);
                List<Theme> themeList = HttpContext.Session.Get<List<Theme>>("themes");
                if (themeList==null)
                {
                    themeList = new List<Theme> { themeModel };
                }
                else if(themeList.Contains(themeModel))
                {
                    return null;
                }
                else
                {
                    themeList.Add(themeModel);
                }
                HttpContext.Session.Set<List<Theme>>("themes", themeList);
                var messages = (from m in db.messages
                                join t in db.themes on m.themeId equals t.id 
                                join u in db.users on m.userSenderId equals u.id
                                where themeList.Select(t=>t.id).Contains(t.id)
                                orderby m.time 
                                select new {userSender=u.login, m.id, m.themeId, m.time, m.text  }).ToArray(); //later
                var session = HttpContext.Session.SessionInfo();
                session.Add("messages", JsonSerializer.Serialize(messages));
                terminalResultBuilder.AddJSFuncModel("updateChats", session);
                return Json(terminalResultBuilder.Build());
            }
            else
            {
                HttpContext.Session.SetInt32("currentThemeId", -1);
                terminalResultBuilder.AddJSFuncInline("addTextToConsole", "theme not found");
                return Json(terminalResultBuilder.Build());
            }
        }
        
        [HttpPost]
        public void deselect(int id)
        {
            try
            {
                var themeList = HttpContext.Session.Get<List<Theme>>("themes");
                var theme = themeList.Find(t => t.id == id);
                themeList.Remove(theme);
                HttpContext.Session.Set<List<Theme>>("themes", themeList);
            }
            catch { }
        }
  
        [HttpPost]
        public IActionResult clear()
        {
            terminalResultBuilder.AddJSFuncInline("clear");
            return Json(terminalResultBuilder.Build());
        }

        [Authorize]
        [HttpPost]
        public IActionResult createTheme(CreateThemeViewModel themeViewModel)
        {
            if (themeViewModel == null)
            {
                terminalResultBuilder.AddAspView(this, "CreateTheme");
                return Json(terminalResultBuilder.Build());
            }
            else
            {
                if (db.themes.Any(t => t.name == themeViewModel.name))
                {
                    ModelState.AddModelError(nameof(themeViewModel.name), "Theme with the same name already exists");
                }
                if (ModelState.IsValid)
                {
                    Theme newTheme = themeViewModel.GetDBModel(db.users.First(u => u.login == User.Identity.Name).id);
                    db.themes.Add(newTheme);
                    db.SaveChanges();
                    terminalResultBuilder.AddAjaxFunc("Terminal/select", new Dictionary<string, string> { { "themeId", newTheme.id.ToString() } });
                    return Json(terminalResultBuilder.Build());

                }
                terminalResultBuilder.AddAspView(this, "CreateTheme", themeViewModel);
                return Json(terminalResultBuilder.Build());
            }
        }
        public IActionResult help()
        {
            foreach (var cmd in cmdTranslator.cmdToAction)
            {
                string outInfo = cmd.Key + ": " + cmd.Value.description;
                terminalResultBuilder.AddJSFuncInline("addTextToConsole", outInfo);
            }
            terminalResultBuilder.AddJSFuncInline("addTextToConsole", "Press button in the left bottom corner to switch between terminal screen and chats.");
            return Json(terminalResultBuilder.Build());
        }
    }
}