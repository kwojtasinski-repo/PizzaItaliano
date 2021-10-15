using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Events
{
    public class AddedPayment : IEvent
    {
        public Guid PaymentId { get; }

        public AddedPayment(Guid paymentId)
        {
            PaymentId = paymentId;
        }
    }
}
