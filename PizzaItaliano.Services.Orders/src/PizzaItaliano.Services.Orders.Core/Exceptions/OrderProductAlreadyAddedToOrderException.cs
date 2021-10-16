using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public class OrderProductAlreadyAddedToOrderException : DomainException
    {
        public override string Code => "order_product_already_added_to_order";
        public Guid OrderId { get; }
        public Guid ProductId { get; }

        public OrderProductAlreadyAddedToOrderException(Guid orderId, Guid productId) : base($"Order with id: '{orderId}' has already product with id: '{productId}'")
        {
            OrderId = orderId;
            ProductId = productId;
        }
    }
}
