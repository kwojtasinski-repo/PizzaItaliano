using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Exceptions
{
    public class PaymentAlreadyExistsException : AppException
    {
        public override string Code => "payment_already_exists";
        public Guid Id { get; }

        public PaymentAlreadyExistsException(Guid id) : base($"Payment with id: '{id}' already exists")
        {
            Id = id;
        }
    }
}
