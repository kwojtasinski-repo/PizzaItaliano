using PizzaItaliano.Services.Orders.Core;
using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Events
{
    public class CreateOrder : IDomainEvent
    {
        public Order Order { get; }

        public CreateOrder(Order order)
        {
            Order = order;
        }
    }
}
