using Convey.CQRS.Events;
using PizzaItaliano.Services.Payments.Application.Events;
using PizzaItaliano.Services.Payments.Application.Services;
using PizzaItaliano.Services.Payments.Core;
using PizzaItaliano.Services.Payments.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Services
{
    internal sealed class EventMapper : IEventMapper
    {
        public IEvent Map(IDomainEvent @event)
        {
            IEvent eventMapped = @event switch
            {
                CreatePayment e => new AddedPayment(e.Payment.Id, e.Payment.OrderId),
                PaymentPaid e => new PaidPayment(e.Payment.Id, e.Payment.OrderId),

                _ => null
            };

            return eventMapped;
        }

        public IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events)
        {
            var eventsMapped = events.Select(e => Map(e));
            return eventsMapped;
        }
    }
}
