using PizzaItaliano.Services.Payments.Core.Entities;

namespace PizzaItaliano.Services.Payments.Core.Events
{
    public class PaymentWithdrawn : IDomainEvent
    {
        public Payment Payment { get; }

        public PaymentWithdrawn(Payment payment)
        {
            Payment = payment;
        }
    }
}
