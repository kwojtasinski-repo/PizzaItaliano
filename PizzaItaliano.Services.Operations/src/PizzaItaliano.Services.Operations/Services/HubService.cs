using PizzaItaliano.Services.Operations.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Services
{
    public class HubService : IHubService
    {
        private readonly IHubWrapper _hubContextWrapper;

        public HubService(IHubWrapper hubContextWrapper)
        {
            _hubContextWrapper = hubContextWrapper;
        }

        public async Task PublishOperationCompletedAsync(OperationDto operation)
            => await _hubContextWrapper.PublishToUserAsync(operation.UserId,
                    "operation_pending",
                    new
                    {
                        id = operation.Id,
                        name = operation.Name
                    }
                );

        public async Task PublishOperationPendingAsync(OperationDto operation)
            => await _hubContextWrapper.PublishToUserAsync(operation.UserId,
                    "operation_completed",
                    new
                    {
                        id = operation.Id,
                        name = operation.Name
                    }
                );

        public async Task PublishOperationRejectedAsync(OperationDto operation)
            => await _hubContextWrapper.PublishToUserAsync(operation.UserId,
                "operation_rejected",
                new
                {
                    id = operation.Id,
                    name = operation.Name,
                    code = operation.Code,
                    reason = operation.Reason
                }
            );
    }
}
