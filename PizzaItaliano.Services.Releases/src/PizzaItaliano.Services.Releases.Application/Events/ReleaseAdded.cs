using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Application.Events
{
    [Contract]
    public class ReleaseAdded : IEvent
    {
        public Guid ReleaseId { get; }
        public Guid OrderId { get; }
        public Guid OrderProductId { get; }
        
        public ReleaseAdded(Guid releaseId, Guid orderId, Guid orderProductId)
        {
            ReleaseId = releaseId;
            OrderId = orderId;
            OrderProductId = orderProductId;
        }
    }
}
