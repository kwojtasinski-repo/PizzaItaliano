using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Events
{
    [Contract]
    public class PaidPayment : IEvent
    {
        public Guid PaymentId { get; }
        public Guid OrderId { get; }

        public PaidPayment(Guid paymentId, Guid orderId)
        {
            PaymentId = paymentId;
            OrderId = orderId;
        }
    }
}
