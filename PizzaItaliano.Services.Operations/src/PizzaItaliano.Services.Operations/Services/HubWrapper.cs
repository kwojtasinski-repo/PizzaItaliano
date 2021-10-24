using Microsoft.AspNetCore.SignalR;
using PizzaItaliano.Services.Operations.Hubs;
using PizzaItaliano.Services.Operations.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Services
{
    public class HubWrapper : IHubWrapper
    {
        private readonly IHubContext<PizzaItalianoHub> _hubContext;

        public HubWrapper(IHubContext<PizzaItalianoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PublishToUserAsync(string userId, string message, object data)
            => await _hubContext.Clients.Group(userId.ToUserGroup()).SendAsync(message, data);

        public async Task PublishToAllAsync(string message, object data)
            => await _hubContext.Clients.All.SendAsync(message, data);
    }
}
