using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Core.Exceptions
{
    public class InvalidPaymentNumberException : DomainException
    {
        public override string Code => "invalid_payment_number";
        public Guid PaymentId { get; }
        public string PaymentNumber { get; }

        public InvalidPaymentNumberException(Guid paymentId, string paymentNumber) : base($"Invalid payment number '{paymentNumber}' for payment with id: '{paymentId}'")
        {
            PaymentId = paymentId;
            PaymentNumber = paymentNumber;
        }
    }
}
