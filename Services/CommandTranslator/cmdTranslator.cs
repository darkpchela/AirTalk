using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AirTalk.Services.CommandTranslator
{
    public class cmdTranslator
    {
        private static Dictionary<string, cmdInfo> cmdToAction { get; set; }
        static cmdTranslator()
        {
                cmdToAction = new Dictionary<string, cmdInfo>();
                cmdToAction.Add("login", new cmdInfo("login", false, true));
                cmdToAction.Add("logout", new cmdInfo("logout", false, true));
                cmdToAction.Add("chatmode", new cmdInfo("chatmode", true, false, new List<string> { "turnedOn" }));
                cmdToAction.Add("select", new cmdInfo("select", false, false, new List<string> {"themeId" }));
        }
        public cmdResponse ReadCommand(string request)
        {
            cmdRequest cmdRequest = new cmdRequest(request);
            try
            {
                var cmdInfo = cmdToAction[cmdRequest.cmdCommand];
                if (cmdInfo.requireParams && !cmdRequest.cmdCommandParams.Any())
                    throw new Exception();
                if (cmdInfo.noParams && cmdRequest.cmdCommandParams.Any())
                    throw new Exception();
                return new cmdResponse(cmdInfo, cmdRequest);
            }
            catch
            {
                string error="~error";
                return cmdResponse.getCustomResponse("error", new Dictionary<string, dynamic> { { "error",3.14 } });
            }
        }
    }
}
