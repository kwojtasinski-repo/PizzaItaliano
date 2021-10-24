using PizzaItaliano.Services.Operations.DTO;
using PizzaItaliano.Services.Operations.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Services
{
    public interface IOperationsService
    {
        event EventHandler<OperationUpdatedEventArgs> OperationUpdated;
        Task<OperationDto> GetAsync(string id);

        Task<(bool updated, OperationDto operation)> TrySetAsync(string id, string userId, string name,
            OperationState state, string code = null, string reason = null);
    }
}
