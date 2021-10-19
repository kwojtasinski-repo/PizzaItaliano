using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class OrderProductNotFoundException : AppException
    {
        public override string Code => "order_product_not_found";
        public Guid OrderProductId { get; }

        public OrderProductNotFoundException(Guid orderProductId) : base($"Order product with id: '{orderProductId}' was not found")
        {
            OrderProductId = orderProductId;
        }
    }
}
