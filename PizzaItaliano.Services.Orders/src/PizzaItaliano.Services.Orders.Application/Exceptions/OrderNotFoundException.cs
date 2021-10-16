using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class OrderNotFoundException : AppException
    {
        public override string Code => "order_not_found";
        public Guid OrderId { get; }

        public OrderNotFoundException(Guid orderId) : base($"Order with id: '{orderId}' was not found")
        {
            OrderId = orderId;
        }
    }
}
