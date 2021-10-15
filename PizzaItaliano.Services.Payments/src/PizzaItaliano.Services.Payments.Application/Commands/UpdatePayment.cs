using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Commands
{
    public class UpdatePayment : ICommand
    {
        public Guid PaymentId { get; set; }
    }
}
