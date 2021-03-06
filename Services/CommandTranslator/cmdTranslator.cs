﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AirTalk.Services.CommandTranslator
{
    public class cmdTranslator
    {
        public static Dictionary<string, cmdInfo> cmdToAction { get; private set; }
        static cmdTranslator()
        {
            cmdToAction = new Dictionary<string, cmdInfo>();
            cmdToAction.Add("login", new cmdInfo(
                "Terminal/login",
                "Type 'login' to get login-form."
                ));

            cmdToAction.Add("logout", new cmdInfo(
                "Terminal/logout",
                "Type 'logout' to logout. Thanks, capitan."
                ));

            //cmdToAction.Add("chatmode", new cmdInfo(
            //    "Terminal/chatmode",
            //    true,
            //    false,
            //    new Dictionary<string, Type> { { "state", typeof(bool) } }
            //    ));

            cmdToAction.Add("select", new cmdInfo(
                "Terminal/select",
                false,
                false,
                new Dictionary<string, Type> { { "themeId", typeof(int) } },
                "Type 'select' to see all themes. Type 'select [id]' to add theme[id] to chats."
                ));

            cmdToAction.Add("clear", new cmdInfo(
                "Terminal/clear",
                "Type 'clear' to clear your terminal screen or remove form."
                ));

            cmdToAction.Add("create", new cmdInfo(
                "Terminal/createTheme",
                "Type 'create' to get theme-cretor form."
                ));

            cmdToAction.Add("reg", new cmdInfo(
                "Terminal/registration",
                "Type 'reg' to get registration form."
                ));

            cmdToAction.Add("help", new cmdInfo(
                "Terminal/help",
                "Type 'help' to see all commands and there description."
                ));
        }
        public cmdResponse ReadCommand(string request)
        {
            string errorMes = "";
            cmdRequest cmdRequest = new cmdRequest(request);
            try
            {
                if (!cmdToAction.ContainsKey(cmdRequest.cmdCommand))
                {
                    errorMes = "unknown command";
                    throw new Exception(errorMes);
                }

                var cmdInfo = cmdToAction[cmdRequest.cmdCommand];
                if (cmdInfo.requireParams && !cmdRequest.cmdCommandParams.Any())
                {
                    errorMes = "command needs parameters";
                    throw new Exception(errorMes);
                }
                if (cmdInfo.noParams && cmdRequest.cmdCommandParams.Any())
                {
                    errorMes = "command does not support params";
                    throw new Exception(errorMes);
                }
                if (cmdRequest.cmdCommandParams.Length < 1)
                    return new cmdResponse(cmdInfo);

                bool paramsMatched= TryMatchParams(cmdInfo, cmdRequest, out Dictionary<string, string> keyParam);
                if (!paramsMatched)
                {
                    errorMes = "command parameters matching error";
                    throw new Exception(errorMes);
                }
                
                return new cmdResponse(cmdInfo, keyParam);
            }
            catch(Exception ex)
            {
                return cmdResponse.getCustomResponse("Terminal/error", new Dictionary<string, string> { { "mes", ex.Message } });
            }
        }
        private bool TryMatchParams(cmdInfo cmdInfo, cmdRequest cmdRequest, out Dictionary<string, string> keyParam)
        {
            var keyType = cmdInfo.paramsKeyType;
            var reqParams = cmdRequest.cmdCommandParams;
            keyParam = new Dictionary<string, string>();

            if (reqParams.Length > keyType.Count)
                return false;

            bool allTypesMatched = false;

            for (int i = 0; i < keyType.Count; i++)
            {
                if (allTypesMatched)
                    break;

                Type typeNeeded = keyType.ElementAt(i).Value;
                for (int j = 0; j < reqParams.Length; j++)
                {
                    try
                    {
                        var check = Convert.ChangeType(reqParams[j], typeNeeded);
                        keyParam.Add(keyType.ElementAt(i).Key, reqParams[j]);

                        if (keyParam.Count == reqParams.Length)
                        {
                            allTypesMatched = true;
                            break;
                        }
                    }
                    catch
                    { continue; }
                }
            }

            if (reqParams.Length > keyParam.Count)
                return false;
            else
                return true;
        }
    }
}
