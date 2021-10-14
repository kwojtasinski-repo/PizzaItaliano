using PizzaItaliano.Services.Payments.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Core.Entities
{
    public class Payment : AggregateRoot
    {
        public string Number { get; private set; }
        public decimal Cost { get; private set; }
        public Guid OrderId { get; private set; }

        public Payment(Guid id, string number, decimal cost, Guid orderId)
        {
            Id = id;
            Number = number;
            Cost = cost;
            OrderId = orderId;
        }

        public static Payment Create(Guid id, string number, decimal cost, Guid orderId)
        {
            var payment = new Payment(id, number, cost, orderId);
            payment.AddEvent(new CreatePayment(payment));
            return payment;
        }
    }
}
