using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class CannotChangeOrderStatusException : AppException
    {
        public override string Code => "cannot_change_order_status";
        public Guid OrderId { get; }

        public CannotChangeOrderStatusException(Guid orderId) : base($"Cannot change order status for order with id: {orderId}")
        {
            OrderId = orderId;
        }
    }
}
