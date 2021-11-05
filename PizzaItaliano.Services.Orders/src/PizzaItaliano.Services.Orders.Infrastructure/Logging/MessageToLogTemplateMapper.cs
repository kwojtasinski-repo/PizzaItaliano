using Convey.Logging.CQRS;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Logging
{
    internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
    {
        public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
        {
            var key = message.GetType();

            return Templates.TryGetValue(key, out var template) ? template : null;
        }

        private static readonly IReadOnlyDictionary<Type, HandlerLogTemplate> Templates = new Dictionary<Type, HandlerLogTemplate>()
        {
            [typeof(AddOrder)] = new HandlerLogTemplate
            {
                Before = "Adding an order with id: {OrderId}",
                After = "Added an order with id: {OrderId}",
                OnError = new Dictionary<Type, string> // w zaleznosci w jakim kontekscie poleci wyjatek
                {
                    [typeof(OrderAlreadyExistsException)] = "Order with id: {OrderId} already exists"
                }
            }
        };
    }
}
