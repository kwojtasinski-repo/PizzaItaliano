using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Commands
{
    [Contract]
    public class SetOrderStatusReady : ICommand
    {
        public Guid OrderId { get; }

        public SetOrderStatusReady(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
