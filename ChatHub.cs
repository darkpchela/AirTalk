using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace AirTalk
{
    public class ChatHub : Hub
    {
        public async Task Create(string product, string connectionId)
        {
            await Clients.AllExcept(connectionId).SendAsync("Notify", $"Добавлено: {product} - {DateTime.Now.ToShortTimeString()}");
            await Clients.Client(connectionId).SendAsync("Notify", $"Ваш товар добавлен!");
        }

    }
}
