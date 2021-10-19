using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public class CannotDeleteOrderProductException : AppException
    {
        public override string Code => "cannot_delete_order_product";
        public Guid OrderId { get; }
        public Guid OrderProductId { get; }
        public int Quantity { get; }

        public CannotDeleteOrderProductException(Guid orderId, Guid orderProductId, int quantity) : base($"Cannot delete order product with id: '{orderProductId}' with quantity '{quantity}' from order with id: {orderId}")
        {
            OrderId = orderId;
            OrderProductId = orderProductId;
            Quantity = quantity;
        }
    }
}
