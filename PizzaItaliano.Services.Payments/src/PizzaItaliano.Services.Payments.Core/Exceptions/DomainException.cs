using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Core.Exceptions
{
    public abstract class DomainException : Exception
    {
        public abstract string Code { get; }

        public DomainException(string message) : base(message)
        {
        }
    }
}
