using Convey.CQRS.Events;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Decorators
{
    internal sealed class OutboxEventHandlerDecorator<T> : IEventHandler<T> where T : class, IEvent
    {
        private readonly IEventHandler<T> _eventHandler;
        private readonly IMessageOutbox _messageOutbox;
        private readonly bool _enabled;
        private readonly string _messageId;

        public OutboxEventHandlerDecorator(IEventHandler<T> eventHandler, IMessageOutbox messageOutbox,
            OutboxOptions outboxOptions, IMessagePropertiesAccessor messagePropertiesAccesor)
        {
            _eventHandler = eventHandler;
            _messageOutbox = messageOutbox;
            _enabled = outboxOptions.Enabled;

            var messageProperties = messagePropertiesAccesor.MessageProperties;
            _messageId = string.IsNullOrWhiteSpace(messageProperties?.MessageId)
                ? Guid.NewGuid().ToString("N")
                : messageProperties.MessageId;

        }

        public Task HandleAsync(T @event)
            => _enabled ? _messageOutbox.HandleAsync(_messageId, () => _eventHandler.HandleAsync(@event))
                        : _eventHandler.HandleAsync(@event);
    }   
}
