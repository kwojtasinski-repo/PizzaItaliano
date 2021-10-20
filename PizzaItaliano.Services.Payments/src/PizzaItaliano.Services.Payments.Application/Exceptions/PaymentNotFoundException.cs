using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Exceptions
{
    public class PaymentNotFoundException : AppException
    {
        public override string Code => "payment_not_found";
        public Guid PaymentId { get; }

        public PaymentNotFoundException(Guid paymentId) : base($"Payment with id: {paymentId} was not found")
        {
            PaymentId = paymentId;
        }
    }
}
