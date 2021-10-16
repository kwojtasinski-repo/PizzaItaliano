using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public class CannotChangeOrderStateException : DomainException
    {
        public override string Code => "cannot_change_order_state";
        public Guid OrderId { get; }
        public OrderStatus CurrentOrderStatus { get; }
        public OrderStatus NewOrderStatus { get; }

        public CannotChangeOrderStateException(Guid orderId, OrderStatus currentOrderStatus, OrderStatus newOrderStatus) : base($"Order with id: '{orderId}' cannot change status from '{currentOrderStatus}' to '{newOrderStatus}'")
        {
            OrderId = orderId;
            CurrentOrderStatus = currentOrderStatus;
            NewOrderStatus = newOrderStatus;
        }
    }
}
