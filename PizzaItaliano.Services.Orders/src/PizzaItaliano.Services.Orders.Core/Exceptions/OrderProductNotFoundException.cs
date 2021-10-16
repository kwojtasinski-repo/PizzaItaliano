using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public class OrderProductNotFoundException : DomainException
    {
        public override string Code => "order_product_not_found";
        public Guid OrderId { get; }
        public Guid OrderProductId { get; }

        public OrderProductNotFoundException(Guid orderId, Guid orderProductId) : base($"Order with id: '{orderId}' doesnt have product with id '{orderProductId}'")
        {
            OrderId = orderId;
            OrderProductId = orderProductId;
        }
    }
}
