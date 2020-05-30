using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirTalk.Services.CommandTranslator
{
    public class cmdResponse
    {
        public string action { get; private set; }
        public Dictionary<string, string> cmdParams { get; private set; }
        private cmdResponse()
        {

        }
        public cmdResponse(cmdInfo cmdInfo):this(cmdInfo, null)
        {
        }
        public cmdResponse(cmdInfo cmdInfo, Dictionary<string, string> cmdParams)
        {
            this.action = cmdInfo.action;
            this.cmdParams = cmdParams;
        }
        public static cmdResponse getCustomResponse(string action, Dictionary<string, string> cmdParams)
        {
            cmdResponse response = new cmdResponse();
            response.action = action;
            response.cmdParams = cmdParams;
            return response;
        }
    }
}
