using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirTalk.Services
{
    public class TerminalResult
    {
        public string type { get; set; }
        public string context { get; set; }
        public string[] inlineParams { get; set; }
        public Dictionary<string, string> contextParams { get; set; }
        public TerminalResult(string type, string context, Dictionary<string, string> keyValues = null)
        {
            this.type = type;
            this.contextParams = keyValues;
            this.context = context;
        }
        public TerminalResult(string type, string context, params string[] _params)
        {
            this.type = type;
            this.inlineParams = _params;
            this.context = context;
        }
    }
}
