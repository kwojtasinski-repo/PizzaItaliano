using Convey.CQRS.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Operations.Types
{
    public class RejectedEvent : IRejectedEvent, IMessage
    {
        public string Reason { get; set; }
        public string Code { get; set; }
    }
}
