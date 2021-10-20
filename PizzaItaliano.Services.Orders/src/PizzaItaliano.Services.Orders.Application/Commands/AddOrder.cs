using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Commands
{
    [Contract]
    public class AddOrder : ICommand
    {
        public Guid OrderId { get; }

        public AddOrder(Guid orderId)
        {
            OrderId = orderId == Guid.Empty ? Guid.NewGuid() : orderId;
        }
    }
}
