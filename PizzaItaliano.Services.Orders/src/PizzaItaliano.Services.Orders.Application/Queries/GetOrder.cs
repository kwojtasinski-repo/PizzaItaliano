using Convey.CQRS.Queries;
using PizzaItaliano.Services.Orders.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Queries
{
    public class GetOrder : IQuery<OrderDto>
    {
        public Guid OrderId { get; set; }
    }
}
