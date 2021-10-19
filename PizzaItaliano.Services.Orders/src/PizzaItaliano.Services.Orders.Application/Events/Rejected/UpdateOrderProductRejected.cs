using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Events.Rejected
{
    [Contract]

    public class UpdateOrderProductRejected : IRejectedEvent
    {
        public Guid OrderProductId { get; }
        public string Reason { get; }
        public string Code { get; }

        public UpdateOrderProductRejected(Guid orderProductId, string reason, string code)
        {
            OrderProductId = orderProductId;
            Reason = reason;
            Code = code;
        }
    }
}
