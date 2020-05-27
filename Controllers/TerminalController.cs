using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using AirTalk.Services.CommandTranslator;

namespace AirTalk.Controllers
{
    public class TerminalController : Controller
    {
        cmdTranslator cmdTranslator;
        public TerminalController(cmdTranslator cmdTranslator)
        {
            this.cmdTranslator = cmdTranslator;
        }
        [HttpPost]
        public IActionResult InitializeCommand(string request)
        {
            var a = new {id=1 };
            var cmdResponse = cmdTranslator.ReadCommand(request);
            if (cmdResponse.cmdParams != null)
            {
                return RedirectToAction(cmdResponse.action, cmdResponse.cmdParams);
            }
            return RedirectToAction(cmdResponse.action);
           
        }
        public IActionResult error(dynamic error)
        {
            if (error!=null)
                return Json((object)error.GetType());
            return Json(false);
        }
        public IActionResult login()
        {
           return RedirectToAction("Login","Account");
        }
        //public IActionResult logout()
        //{

        //}
        //public IActionResult chatmode()
        //{

        //}
        //public IActionResult select()
        //{

        //}
    }
}