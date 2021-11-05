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
            },
            [typeof(AddOrderProduct)] = new HandlerLogTemplate
            {
                Before = "Adding an order product with id: {OrderProductId}",
                After = "Added an order product with id: {OrderProductId}",
                OnError = new Dictionary<Type, string>
                {
                    [typeof(CannotAddOrderProductException)] = "Order product with id: {OrderProductId} cannot be added",
                    [typeof(OrderNotFoundException)] = "Order with id: {OrderId} not found",
                    [typeof(ProductNotFoundException)] = "Product with id: {ProductId} not found"
                }
            },
            [typeof(DeleteOrderProduct)] = new HandlerLogTemplate
            {
                Before = "Deleting an order product with id: {OrderProductId}",
                After = "Deleted an order product with id: {OrderProductId}",
                OnError = new Dictionary<Type, string>
                {
                    [typeof(CannotDeleteOrderProductException)] = "Order product with id: {OrderProductId} cannot be deleted",
                    [typeof(OrderNotFoundException)] = "Order with id: {OrderId} not found",
                    [typeof(OrderProductNotFoundException)] = "Order product with id: {OrderProductId} not found"
                }
            },
            [typeof(SetOrderStatusReady)] = new HandlerLogTemplate
            {
                Before = "Setting status ready for order with id: {OrderId}",
                After = "Set status ready for order with id: {OrderId}",
                OnError = new Dictionary<Type, string>
                {
                    [typeof(OrderNotFoundException)] = "Order with id: {OrderId} not found",
                    [typeof(CannotChangeOrderStatusException)] = "Cannot change status for order with id: {OrderId}"
                }
            }
        };
    }
}
