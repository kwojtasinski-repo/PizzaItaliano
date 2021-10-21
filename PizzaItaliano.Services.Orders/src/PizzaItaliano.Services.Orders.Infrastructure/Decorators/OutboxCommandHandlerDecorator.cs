using Convey.CQRS.Commands;
using Convey.MessageBrokers;
using Convey.MessageBrokers.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Decorators
{
    internal sealed class OutboxCommandHandlerDecorator<T> : ICommandHandler<T> where T : class, ICommand
    {
        private readonly ICommandHandler<T> _commandHandler;
        private readonly IMessageOutbox _messageOutbox;
        private readonly bool _enabled;
        private readonly string _messageId;

        public OutboxCommandHandlerDecorator(ICommandHandler<T> commandHandler, IMessageOutbox messageOutbox, 
                OutboxOptions outboxOptions, IMessagePropertiesAccessor messagePropertiesAccesor)
        {
            _commandHandler = commandHandler;
            _messageOutbox = messageOutbox;
            _enabled = outboxOptions.Enabled;

            var messageProperties = messagePropertiesAccesor.MessageProperties;
            _messageId = string.IsNullOrWhiteSpace(messageProperties?.MessageId)
                        ? Guid.NewGuid().ToString("N")
                        : messageProperties.MessageId;
        }

        public Task HandleAsync(T command)
            => _enabled
                ? _messageOutbox.HandleAsync(_messageId, () => _commandHandler.HandleAsync(command))
                : _commandHandler.HandleAsync(command);
    }
}
