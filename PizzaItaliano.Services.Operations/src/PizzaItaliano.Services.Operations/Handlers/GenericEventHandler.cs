using Convey.CQRS.Events;
using Convey.MessageBrokers;
using PizzaItaliano.Services.Operations.Infrastructure;
using PizzaItaliano.Services.Operations.Services;
using PizzaItaliano.Services.Operations.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Handlers
{
    public class GenericEventHandler<T> : IEventHandler<T> where T : class, IEvent
    {
        private readonly ICorrelationContextAccessor _contextAccessor;
        private readonly IMessagePropertiesAccessor _messagePropertiesAccessor;
        private readonly IOperationsService _operationsService;
        private readonly IHubService _hubService;

        public GenericEventHandler(ICorrelationContextAccessor contextAccessor,
            IMessagePropertiesAccessor messagePropertiesAccessor,
            IOperationsService operationsService, IHubService hubService)
        {
            _contextAccessor = contextAccessor;
            _messagePropertiesAccessor = messagePropertiesAccessor;
            _operationsService = operationsService;
            _hubService = hubService;
        }

        public async Task HandleAsync(T @event)
        {
            var messageProperties = _messagePropertiesAccessor.MessageProperties;
            var correlationId = messageProperties?.CorrelationId;
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                return;
            }

            var context = _contextAccessor.GetCorrelationContext();
            var name = string.IsNullOrWhiteSpace(context?.Name) ? @event.GetType().Name : context.Name;
            var userId = context?.User?.Id;
            var state = OperationState.Completed;
            var (updated, operation) = await _operationsService.TrySetAsync(correlationId, userId, name, state);
            if (!updated)
            {
                return;
            }

            switch (state)
            {
                case OperationState.Pending:
                    await _hubService.PublishOperationPendingAsync(operation);
                    break;
                case OperationState.Completed:
                    await _hubService.PublishOperationCompletedAsync(operation);
                    break;
                case OperationState.Rejected:
                    await _hubService.PublishOperationRejectedAsync(operation);
                    break;
                default:
                    throw new ArgumentException($"Invalid operation state: {state}", nameof(state));
            }
        }
    }
}
