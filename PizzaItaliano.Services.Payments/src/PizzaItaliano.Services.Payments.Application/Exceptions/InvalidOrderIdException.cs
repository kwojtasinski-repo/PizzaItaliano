using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Exceptions
{
    public class InvalidOrderIdException : AppException
    {
        public override string Code => "invalid_order_id";
        public Guid Id { get; }

        public InvalidOrderIdException(Guid id) : base($"Invalid Order with id: '{id}'")
        {
            Id = id;
        }
    }
}
