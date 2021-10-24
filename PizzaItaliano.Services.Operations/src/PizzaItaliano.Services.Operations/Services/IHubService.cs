using PizzaItaliano.Services.Operations.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Services
{
    public interface IHubService
    {
        Task PublishOperationPendingAsync(OperationDto operation);
        Task PublishOperationCompletedAsync(OperationDto operation);
        Task PublishOperationRejectedAsync(OperationDto operation);
    }
}
