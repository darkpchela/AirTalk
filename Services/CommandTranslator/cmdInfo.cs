using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirTalk.Services.CommandTranslator
{
    public class cmdInfo
    {
        public string action { get; private set; }
        public bool requireParams { get; private set; }
        public bool noParams { get; private set; }
        public Dictionary<string, Type> paramsKeyType { get; private set; }
        public string description { get; private set; }

        public cmdInfo(string action, string description="No description") : this(action, false, true, null,description) { }
        public cmdInfo(string action, bool requireParams, bool noParamsl,  Dictionary<string, Type> paramsKeyType = null, string description = "No description")
        {
            this.noParams = noParams;
            this.action = action;
            this.requireParams = requireParams;
            this.paramsKeyType = paramsKeyType;
            this.description = description;
        }
    }
}
