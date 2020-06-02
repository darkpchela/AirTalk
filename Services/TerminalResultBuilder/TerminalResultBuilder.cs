using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirTalk.Services.TerminalResultBuilder
{
    public class TerminalResultBuilder
    {
        private List<TerminalResult> jsFuncs;
        private List<TerminalResult> ajaxFuncs;
        private List<TerminalResult> aspViews;
        public void AddJSFunc(string funcName, Dictionary<string, string> funcParams=null)
        {
            if (jsFuncs == null)
                jsFuncs = new List<TerminalResult>();

            TerminalResult terminalSubResult = new TerminalResult("js", funcName ,funcParams);
            jsFuncs.Add(terminalSubResult);
        }
        public void AddAjaxFunc(string aspAction, Dictionary<string, string> actionParams=null)
        {
            if (ajaxFuncs == null)
                ajaxFuncs = new List<TerminalResult>();

            TerminalResult terminalSubResult = new TerminalResult("ajax",aspAction ,actionParams);
            ajaxFuncs.Add(terminalSubResult);
        }
        public void AddAspView(string html)
        {
            if (aspViews == null)
                aspViews = new List<TerminalResult>();

            TerminalResult terminalSubResult = new TerminalResult("view", html);
            aspViews.Add(terminalSubResult);
        }
        public TerminalResult[] Build()
        {
            List<TerminalResult> allResults = new List<TerminalResult>();
            if (aspViews != null)
                allResults.AddRange(aspViews);
            if (jsFuncs != null)
                allResults.AddRange(jsFuncs);
            if (ajaxFuncs != null)
                allResults.AddRange(ajaxFuncs);

            return allResults.ToArray();
        }

    }
}
