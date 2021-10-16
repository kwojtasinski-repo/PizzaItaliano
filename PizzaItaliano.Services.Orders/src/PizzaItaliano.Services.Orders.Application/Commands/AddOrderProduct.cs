using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Commands
{
    public class AddOrderProduct : ICommand
    {
        public Guid OrderId { get; }
        public Guid OrderProductId { get; }
        public Guid ProductId { get; }
        public int Quantity { get; set; }

        public AddOrderProduct(Guid orderId, Guid orderProductId, Guid productId, int quantity)
        {
            OrderId = orderId;
            OrderProductId = orderProductId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
