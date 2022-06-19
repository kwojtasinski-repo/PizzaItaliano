using Microsoft.AspNetCore.SignalR;
using PizzaItaliano.Services.Operations.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Hubs
{
    public class PizzaItalianoHub : Hub
    {
        public async Task InitializeAsync()
        {
            try
            {
                // version with users
                /*var payload = _jwtHandler.GetTokenPayload(token);
                if (payload is null)
                {
                    await DisconnectAsync();

                    return;
                }

                var group = Guid.Parse(payload.Subject).ToUserGroup();*/
                var group = "".ToUserGroup();
                await Groups.AddToGroupAsync(Context.ConnectionId, group);
                await ConnectAsync();
            }
            catch
            {
                await DisconnectAsync();
            }
        }

        private async Task ConnectAsync()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("connected");
        }

        private async Task DisconnectAsync()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("disconnected");
        }
    }
}
