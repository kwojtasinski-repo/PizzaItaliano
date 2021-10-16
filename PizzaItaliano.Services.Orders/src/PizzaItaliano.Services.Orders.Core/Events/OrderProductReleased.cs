using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Events
{
    public class OrderProductReleased : IDomainEvent
    {
        public OrderProduct OrderProduct { get; }

        public OrderProductReleased(OrderProduct orderProduct)
        {
            OrderProduct = orderProduct;
        }
    }
}
