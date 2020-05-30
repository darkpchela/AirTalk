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
        public Dictionary<string, Type> paramsKeyType { get; private set; }//Testing

        public cmdInfo(string action, bool requireParams, bool noParams)
        {
            this.action = action;
            this.requireParams = requireParams;
            this.noParams = noParams;
            paramsKeyType = null;
        }
        public cmdInfo(string action, bool requireParams, bool noParams, Dictionary<string, Type> paramsKeyType)
        {
            this.noParams = noParams;
            this.action = action;
            this.requireParams = requireParams;
            this.paramsKeyType = paramsKeyType;
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
