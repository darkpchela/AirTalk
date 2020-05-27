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
        public List<string> acceptableParams { get; private set; }
        public Dictionary<string, Type> acceptableParamsT { get; private set; }//Testing

        public cmdInfo(string action, bool requireParams, bool noParams)
        {
            acceptableParams=null;
        }
        public cmdInfo(string action, bool requireParams, bool noParams, List<string> paramsKeys)
        {
            this.noParams = noParams;
            this.action = action;
            this.requireParams = requireParams;
            this.acceptableParams = paramsKeys;
        }


        //public List<string> commandParams { get; private set; } //?
        //public void AddCommandParams(string cmdParam)
        //{
        //    commandParams.Add(cmdParam);
        //}//?
        //public void AddCommandParams(IEnumerable<string> cmdParams)
        //{
        //    commandParams.AddRange(cmdParams);
        //}//?
    }
}
