using PizzaItaliano.Services.Orders.Core.Entities;
using System;

namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class CannotDeleteOrderException : AppException
    {
        public override string Code => "cannot_delete_order";
        public Guid OrderId { get; }
        public OrderStatus OrderStatus { get; }

        public CannotDeleteOrderException(Guid orderId, OrderStatus orderStatus) : base($"Cannot delete order with id: '{orderId}' and status: '{orderStatus}'")
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
        }
    }
}
