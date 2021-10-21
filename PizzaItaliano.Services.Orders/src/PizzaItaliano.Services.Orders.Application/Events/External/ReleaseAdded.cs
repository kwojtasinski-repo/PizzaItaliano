using Convey.CQRS.Events;
using Convey.MessageBrokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Events.External
{
    [Message("release")] // binding do odpowiedniej wymiany
    public class ReleaseAdded : IEvent
    {
        public Guid ReleaseId { get; set; }
        public Guid OrderId { get; set; }
        public Guid OrderProductId { get; set; }

        public ReleaseAdded()
        {

        }

        // Problem z deserializacja system json
        // Each parameter in constructor 'Void .ctor(System.Guid, System.Guid, System.Guid)' on type 
        // must bind to an object property or field on deserialization. Each parameter name must match with a property or field on the object. The match can be case-insensitive.
        public ReleaseAdded(Guid releaseId, Guid orderId, Guid orderPorductId)
        {
            ReleaseId = releaseId;
            OrderId = orderId;
            OrderProductId = orderPorductId;
        }
    }
}
