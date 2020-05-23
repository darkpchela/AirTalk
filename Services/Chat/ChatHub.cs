using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using AirTalk.Models.DBModels;
using Microsoft.AspNetCore.Http;


namespace AirTalk
{
    public class ChatHub : Hub
    {
        ChatLogger chatLogger;

        public ChatHub( ChatLogger chatLogger)
        {
            this.chatLogger = chatLogger;
        }

        public async Task PublicSingle(string userName, string message, string connectionId)
        {
            var groupId = Context.GetHttpContext().Session.GetInt32("currentThemeId").ToString();
            await Clients.Group(groupId).SendAsync("GetMessage", userName, message);
        }
        public async Task PublicAll(string userName, string message)
        {
            await Clients.All.SendAsync("GetMessage", userName, message);
        }
        public override async Task OnConnectedAsync()
        {
            var context = Context.GetHttpContext();
            chatLogger.Connected(context);
            await Groups.AddToGroupAsync(Context.ConnectionId, context.Session.GetInt32("currentThemeId").ToString());
            await base.OnConnectedAsync();
            //GetChatBinderInfo();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var context = Context.GetHttpContext();
            chatLogger.Disconnected(context);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, context.Session.GetInt32("currentThemeId").ToString());
            await base.OnDisconnectedAsync(exception);
            //GetChatBinderInfo();
        }
    }
}
