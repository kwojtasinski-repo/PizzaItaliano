using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Application.Commands
{
    [Contract]
    public class PayFromPayment : ICommand
    {
        public Guid PaymentId { get; set; }
    }
}
