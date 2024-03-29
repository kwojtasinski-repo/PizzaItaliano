﻿using PizzaItaliano.Services.Orders.Core;
using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Events
{
    public class OrderCreated : IDomainEvent
    {
        public Order Order { get; }

        public OrderCreated(Order order)
        {
            Order = order;
        }
    }
}
