using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Exceptions
{
    public class PaymentForOrderNotFoundException : AppException
    {
        public override string Code => "payment_for_order_not_found";
        public Guid OrderId { get; }

        public PaymentForOrderNotFoundException(Guid orderId) : base($"Payment for order with id: {orderId} was not found")
        {
            OrderId = orderId;
        }
    }
}
