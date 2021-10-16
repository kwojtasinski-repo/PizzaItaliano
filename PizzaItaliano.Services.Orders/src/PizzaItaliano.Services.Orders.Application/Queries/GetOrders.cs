using Convey.CQRS.Queries;
using PizzaItaliano.Services.Orders.Application.DTO;
using PizzaItaliano.Services.Orders.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Queries
{
    public class GetOrders : IQuery<IEnumerable<OrderDto>>
    {
        public OrderStatus? OrderStatus { get; set; }
    }
}
