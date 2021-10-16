using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Core.Exceptions
{
    public abstract class DoaminException : Exception
    {
        public abstract string Code { get; }

        protected DoaminException(string message) : base(message)
        {
        }
    }
}
