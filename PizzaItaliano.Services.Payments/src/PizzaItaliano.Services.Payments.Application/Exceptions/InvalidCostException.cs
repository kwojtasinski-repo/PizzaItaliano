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
        public decimal Cost { get; }

        public InvalidCostException(decimal cost) : base($"Invalid cost: '{cost}'")
        {
            Cost = cost;
        }
    }
}
