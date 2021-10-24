using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PizzaItaliano.Services.Operations.DTO;
using PizzaItaliano.Services.Operations.Infrastructure;
using PizzaItaliano.Services.Operations.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Services
{
    public class OperationsService : IOperationsService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RequestsOptions _requestsOptions;

        public OperationsService(IDistributedCache distributedCache, RequestsOptions requestsOptions)
        {
            _distributedCache = distributedCache;
            _requestsOptions = requestsOptions;
        }

        public event EventHandler<OperationUpdatedEventArgs> OperationUpdated;

        public async Task<OperationDto> GetAsync(string id)
        {
            var operation = await _distributedCache.GetStringAsync(GetKey(id));

            return string.IsNullOrWhiteSpace(operation) ? null : JsonConvert.DeserializeObject<OperationDto>(operation);
        }

        public async Task<(bool updated, OperationDto operation)> TrySetAsync(string id, string userId, string name, OperationState state, string code = null, string reason = null)
        {
            var operation = await GetAsync(id);
            if (operation is null)
            {
                operation = new OperationDto();
            }
            else if (operation.State == OperationState.Completed || operation.State == OperationState.Rejected)
            {
                return (false, operation);
            }

            operation.Id = id;
            operation.UserId = userId ?? string.Empty;
            operation.Name = name;
            operation.State = state;
            operation.Code = code ?? string.Empty;
            operation.Reason = reason ?? string.Empty;
            await _distributedCache.SetStringAsync(GetKey(id),
                JsonConvert.SerializeObject(operation),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(_requestsOptions.ExpirySeconds)
                });

            OperationUpdated?.Invoke(this, new OperationUpdatedEventArgs(operation));

            return (true, operation);
        }

        private static string GetKey(string id) => $"requests:{id}";
    }
}
