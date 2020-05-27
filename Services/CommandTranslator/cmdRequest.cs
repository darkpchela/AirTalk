using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirTalk.Services.CommandTranslator
{
    public class cmdRequest
    {
        public readonly string cmdCommand;
        public readonly string[] cmdCommandParams;
        public cmdRequest(string request)
        {
            string[] temp = request.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            cmdCommandParams = temp.Skip(1).ToArray();
            cmdCommand = temp[0];
        }
    }
}
