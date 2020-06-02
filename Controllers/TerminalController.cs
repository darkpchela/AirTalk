using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using AirTalk.Services.CommandTranslator;
using AirTalk.Services.TerminalResultBuilder;
using Microsoft.Extensions.Logging;
using AirTalk.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace AirTalk.Controllers
{
    public class TerminalController : Controller
    {
        cmdTranslator cmdTranslator;
        ILogger<TerminalController> logger;
        ICompositeViewEngine viewEngine;
        TerminalResultBuilder terminalResultBuilder;
        public TerminalController(cmdTranslator cmdTranslator, ILogger<TerminalController> logger,
            ICompositeViewEngine viewEngine, TerminalResultBuilder terminalResultBuilder)
        {
            this.cmdTranslator = cmdTranslator;
            this.logger = logger;
            this.viewEngine = viewEngine;
            this.terminalResultBuilder = new TerminalResultBuilder();
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
            terminalResultBuilder.AddJSFunc("addTextToConsole", new Dictionary<string, string> { { "message", mes } });
            return Json(terminalResultBuilder.Build());
        }
        [HttpPost]
        public async Task<IActionResult> login()
        {
            string view = await RenderPartialViewToString("SignIn");
            terminalResultBuilder.AddAspView(view);
            return Json(terminalResultBuilder.Build());
        }

        [HttpPost]
        public IActionResult logout()
        {
            return RedirectToAction("Logout", "Account");
        }

        //[HttpPost]
        //public IActionResult select(int? themeId)
        //{
        //    if (themeId == null)
        //        return RedirectToAction("SelectTheme", "Main");
        //    else
        //        return RedirectToAction("SelectTheme", "Main", themeId);
        //}
        [HttpPost]
        public IActionResult clear()
        {
            terminalResultBuilder.AddJSFunc("clear");
            return Json(terminalResultBuilder.Build());
        }
        //class SubTerminalResult
        //{
        //    public bool isJsMethod { get; set; }
        //    public bool isView { get; set; }
        //    public string context { get; set; }
        //    public SubTerminalResult(string context)
        //    {
        //        this.isJsMethod = false;
        //        this.context = context;
        //    }
        //}
        //class TerminalResultAjax
        //{
        //    public string action { get; set; }
        //    public Dictionary<string,string> keyParams { get; set; }
        //    public TerminalResultAjax( string action, Dictionary<string,string> keyParams)
        //    {
        //        this.action = action;
        //        this.keyParams = keyParams;
        //    }
        //}
        private async Task<string> RenderPartialViewToString(string viewName, object model=null)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;

            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                ViewEngineResult viewResult =
                    viewEngine.FindView(ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }
    }
}