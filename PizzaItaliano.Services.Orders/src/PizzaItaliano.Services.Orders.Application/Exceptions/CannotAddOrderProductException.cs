using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class CannotAddOrderProductException : AppException
    {
        public override string Code => "cannot_add_order_product";
        public Guid OrderId { get; }
        public Guid OrderProductId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; }

        public CannotAddOrderProductException(Guid orderId, Guid orderProductId, Guid productId, int quantity) : base($"Cannot add order product with id: '{orderProductId}' with product id: '{productId}' and quantity '{quantity}' to order with id: {orderId}")
        {
            OrderId = orderId;
            OrderProductId = orderProductId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
