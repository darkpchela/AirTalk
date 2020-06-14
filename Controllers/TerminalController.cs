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
        public IActionResult select(int? themeId)
        {
            if (themeId==null)
            {
                var themes = db.themes.OrderBy(t => t.creationTime).Take(10);
                foreach(var t in themes)
                {
                    terminalResultBuilder.AddJSFuncInline("addTextToConsole", t.name);
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
        //private async Task<string> RenderPartialViewToString(string viewName, object model=null)
        //{
        //    if (string.IsNullOrEmpty(viewName))
        //        viewName = ControllerContext.ActionDescriptor.ActionName;

        //    ViewData.Model = model;

        //    using (var writer = new StringWriter())
        //    {
        //        ViewEngineResult viewResult =
        //            viewEngine.FindView(ControllerContext, viewName, false);

        //        ViewContext viewContext = new ViewContext(
        //            ControllerContext,
        //            viewResult.View,
        //            ViewData,
        //            TempData,
        //            writer,
        //            new HtmlHelperOptions()
        //        );

        //        await viewResult.View.RenderAsync(viewContext);

        //        return writer.GetStringBuilder().ToString();
        //    }
        //}
    }
}