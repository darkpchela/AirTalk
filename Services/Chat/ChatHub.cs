using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using AirTalk.Models.DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AirTalk.Services;
using Microsoft.EntityFrameworkCore;


namespace AirTalk
{
    public class ChatHub : Hub
    {
        MainDbContext db;
        ILogger<ChatHub> logger;
        public ChatHub(ILogger<ChatHub> logger, MainDbContext db)
        {
            this.logger = logger;
            this.db = db;
        }

        [Authorize]
        public async Task PublicSingle(string themeId, string userName, string message )
        {
            if (themeId == null)
                return;
            Message mes = new Message();
            mes.text = message;
            mes.themeId = Convert.ToInt32(themeId);
            var userSender = db.users.First(u=>u.login==userName);
            mes.userSenderId = userSender.id;
            mes.time = DateTime.UtcNow;
            db.messages.Add(mes);
            await db.SaveChangesAsync();
            var messageId = mes.id;
            await Clients.Group(themeId).SendAsync("getMessageR", themeId, userName, message, messageId);
        }
        [Authorize]
        public async Task PublicAll(string userName, string message)
        {
            await Clients.All.SendAsync("getMessageR", userName, message);
        }
        public async Task Close(string themeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, themeId);
        }
        public async Task Open(string themeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, themeId);
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var context = Context.GetHttpContext();
            await base.OnDisconnectedAsync(exception);
        }
    }
}
