using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Exceptions
{
    public class CannotUpdatePaymentStatusException : AppException
    {
        public override string Code => "cannot_update_payment";
        public Guid PaymentId { get; }

        public CannotUpdatePaymentStatusException(Guid paymentId) : base($"Cannot update payment with id: {paymentId}")
        {
            PaymentId = paymentId;
        }
    }
}
