using PizzaItaliano.Services.Payments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Core.Events
{
    public class PaymentPaid : IDomainEvent
    {
        public Payment Payment { get; }

        public PaymentPaid(Payment payment)
        {
            Payment = payment;
        }
    }
}
