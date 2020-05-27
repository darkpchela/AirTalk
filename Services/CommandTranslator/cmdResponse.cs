using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirTalk.Services.CommandTranslator
{
    public class cmdResponse
    {
        public string action { get; private set; }
        public Dictionary<string, dynamic> cmdParams { get; private set; }
        private cmdResponse()
        {

        }
        public cmdResponse(cmdInfo cmdInfo, cmdRequest cmdRequest)
        {
            this.action = cmdInfo.action;

            //cmdPrams = cmdRequest.cmdCommandParams;
        }
        public static cmdResponse getCustomResponse(string action, Dictionary<string, dynamic> cmdParams)
        {
            cmdResponse response = new cmdResponse();
            response.action = action;
            response.cmdParams = cmdParams;
            return response;
        }
    }
}
