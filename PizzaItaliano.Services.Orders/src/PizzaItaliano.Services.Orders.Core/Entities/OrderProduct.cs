using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Entities
{
    public class OrderProduct
    {
        public Guid Id { get; private set; }
        public decimal Quantity { get; private set; }
        public Guid OrderId { get; private set; }
        public OrderProductStatus OrderProductStatus { get; private set; }
    }
}
