using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Services
{
    public interface IHubWrapper
    {
        Task PublishToUserAsync(string userId, string message, object data);
        Task PublishToAllAsync(string message, object data);
    }
}
