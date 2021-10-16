using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Application.Exceptions
{
    public abstract class AppException : Exception
    {
        public abstract string Code { get; }

        public AppException(string message) : base(message)
        {
        }
    }
}
