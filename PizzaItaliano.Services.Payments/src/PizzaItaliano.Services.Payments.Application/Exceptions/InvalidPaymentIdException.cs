using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Exceptions
{
    public class InvalidPaymentIdException : AppException
    {
        public override string Code => "invalid_payment_id";
        public Guid PaymentId { get; }

        public InvalidPaymentIdException(Guid paymentId) : base($"Invalid payment id: {paymentId}")
        {
            PaymentId = paymentId;
        }

        public InvalidPaymentIdException() : base($"Invalid PaymentId")
        {
            PaymentId = Guid.Empty;
        }
    }
}
