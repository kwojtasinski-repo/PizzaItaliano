using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Exceptions
{
    public class InvalidCostException : AppException
    {
        public override string Code => "invalid_cost";
        public Guid PaymentId { get; }
        public decimal Cost { get; }

        public InvalidCostException(Guid paymentId, decimal cost) : base($"Invalid cost: '{cost}' for payment with id: '{paymentId}'")
        {
            PaymentId = paymentId;
            Cost = cost;
        }
    }
}
