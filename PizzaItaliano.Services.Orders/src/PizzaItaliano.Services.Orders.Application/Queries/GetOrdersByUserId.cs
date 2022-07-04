using Convey.CQRS.Queries;
using PizzaItaliano.Services.Orders.Application.DTO;
using System;
using System.Collections.Generic;

namespace PizzaItaliano.Services.Orders.Application.Queries
{
    public class GetOrdersByUserId : IQuery<IEnumerable<OrderDto>>
    {
    }
}
