using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using AirTalk.Services.CommandTranslator;
using Microsoft.Extensions.Logging;
using AirTalk.Models.ViewModels;
using System.Web;

namespace AirTalk.Controllers
{
    public class TerminalController : Controller
    {
        cmdTranslator cmdTranslator;
        ILogger<TerminalController> logger;
        public TerminalController(cmdTranslator cmdTranslator, ILogger<TerminalController> logger)
        {
            this.cmdTranslator = cmdTranslator;
            this.logger = logger;

        }

        [HttpPost]
        public IActionResult InitializeCommand(string request)
        {
            var cmdResponse = cmdTranslator.ReadCommand(request);
            //if (cmdResponse.cmdParams != null)
            //{
            //    return RedirectToAction(cmdResponse.action, cmdResponse.cmdParams);
            //}
            //return RedirectToAction(cmdResponse.action);
            if (cmdResponse.cmdParams!=null)
            {
                return Json(new TerminalResultAjax(cmdResponse.action, cmdResponse.cmdParams));
            }
            return Json(new TerminalResultAjax(cmdResponse.action, cmdResponse.cmdParams));



        }
        [HttpPost]
        public IActionResult error(string mes)
        {
            return Json(new TerminalResult(mes));
        }
        [HttpPost]
        public IActionResult login()
        {
            return PartialView("SignIn");
        }
        //public IActionResult logout(bool? mode)
        //{

        //}
        [HttpPost]
        public IActionResult select(int? themeId)
        {
            if (themeId == null)
                return RedirectToAction("SelectTheme", "Main");
            else
                return RedirectToAction("SelectTheme", "Main", themeId);
        }
        [HttpPost]
        public IActionResult clear()
        {
            var result = new TerminalResult("clear") { isJsMethod = true };
            return Json(result);
        }
        class TerminalResult
        {
            public bool isJsMethod { get; set; }
            public bool isView { get; set; }
            public string context { get; set; }
            public TerminalResult(string context)
            {
                this.isJsMethod = false;
                this.context = context;
            }
        }
        class TerminalResultAjax
        {
            public string action { get; set; }
            public Dictionary<string,string> keyParams { get; set; }
            public TerminalResultAjax( string action, Dictionary<string,string> keyParams)
            {
                this.action = action;
                this.keyParams = keyParams;
            }
        }
    }
}