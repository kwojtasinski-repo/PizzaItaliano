using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Events
{
    public class OrderStateChanged : IDomainEvent
    {
        public Order OrderBeforeChange { get; }
        public Order OrderAfterChange { get; }

        public OrderStateChanged(Order orderBeforeChange, Order orderAfterChange)
        {
            OrderBeforeChange = orderBeforeChange;
            OrderAfterChange = orderAfterChange;
        }
    }
}
