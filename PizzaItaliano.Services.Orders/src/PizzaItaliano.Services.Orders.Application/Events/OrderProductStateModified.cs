using Convey.CQRS.Events;
using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Events
{
    [Contract]
    public class OrderProductStateModified : IEvent
    {
        public Guid OrderProductId { get; }
        public OrderProductStatus OrderProductStatusBeforeChange { get; }
        public OrderProductStatus OrderProductStatusAfterChange { get; }

        public OrderProductStateModified(Guid orderProdutcId, OrderProductStatus orderProductStatusBeforeChange, OrderProductStatus orderProductStatusAfterChange)
        {
            OrderProductId = orderProdutcId;
            OrderProductStatusBeforeChange = orderProductStatusBeforeChange;
            OrderProductStatusAfterChange = orderProductStatusAfterChange;
        }
    }
}
