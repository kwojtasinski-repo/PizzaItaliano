using Convey.MessageBrokers.RabbitMQ;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Events.Rejected;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Exceptions
{
    public class ExceptionToMessageMapper : IExceptionToMessageMapper
    {
        public object Map(Exception exception, object message)
            => exception switch
            {
                // App Exceptions
                OrderAlreadyExistsException ex => message switch
                {
                    AddOrder command => new AddOrderRejected(command.OrderId, ex.Message, ex.Code),
                    _ => null
                },
                OrderNotFoundException ex => message switch
                {
                    AddOrderProduct command => new AddOrderProductRejected(command.OrderProductId, ex.Message, ex.Code),
                    _ => null
                },
                ProductNotFoundException ex => message switch
                {
                    AddOrderProduct command => new AddOrderProductRejected(command.OrderProductId, ex.Message, ex.Code),
                    _ => null
                },
                CannotAddOrderProductException ex => message switch
                {
                    AddOrderProduct command => new AddOrderProductRejected(command.OrderProductId, ex.Message, ex.Code),
                    _ => null
                },
                Application.Exceptions.CannotDeleteOrderProductException ex => message switch
                {
                    DeleteOrderProduct command => new DeleteOrderProductRejected(command.OrderProductId, ex.Message, ex.Code),
                    _ => null
                },
                Application.Exceptions.OrderProductNotFoundException ex => message switch
                {
                    DeleteOrderProduct command => new DeleteOrderProductRejected(command.OrderProductId, ex.Message, ex.Code),
                    _ => null
                },
                CannotChangeOrderStatusException ex => new UpdateOrderRejected(ex.OrderId, ex.Message, ex.Code),

                // Domain Exceptions
                CannotChangeOrderProductStateException ex => new UpdateOrderProductRejected(ex.OrderProductId, ex.Message, ex.Code),
                CannotChangeOrderStateException ex => new UpdateOrderRejected(ex.OrderId, ex.Message, ex.Code),
                CannotChangeOrderStateWhenProductsNotReleasedException ex => new UpdateOrderRejected(ex.OrderId, ex.Message, ex.Code),
                CannotUpdateOrderCostException ex => new UpdateOrderRejected(ex.OrderId, ex.Message, ex.Code),
                InvalidOrderProductCostException ex => new AddOrderProductRejected(ex.OrderProductId, ex.Message, ex.Code),
                InvalidOrderProductQuantityException ex => new AddOrderProductRejected(ex.OrderProductId, ex.Message, ex.Code),
                OrderProductAlreadyAddedToOrderException ex => message switch 
                { 
                    AddOrderProduct command => new AddOrderProductRejected(command.OrderProductId, ex.Message, ex.Code),
                    _ => null
                },
                Core.Exceptions.OrderProductNotFoundException ex => new DeleteOrderProductRejected(ex.OrderProductId, ex.Message, ex.Code),
                Core.Exceptions.CannotDeleteOrderProductException ex => new DeleteOrderProductRejected(ex.OrderProductId, ex.Message, ex.Code),
                InvalidOrderCostException ex => new AddOrderRejected(ex.OrderId, ex.Message, ex.Code),
                InvalidOrderNumberException ex => new AddOrderRejected(ex.OrderId, ex.Message, ex.Code),

                // other unforeseen exceptions 
                _ => null
            };
    }
}
