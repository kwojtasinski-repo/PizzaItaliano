using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Core.Exceptions
{
    public class InvalidPaymentCostException : DomainException
    {
        public override string Code => "invalid_payment_cost";
        public Guid PaymentId { get; }
        public decimal Cost { get; }

        public InvalidPaymentCostException(Guid paymentId, decimal cost): base($"Invalid cost: '{cost}' for product with id: '{paymentId}'")
        {
            PaymentId = paymentId;
            Cost = cost;
        }
    }
}
