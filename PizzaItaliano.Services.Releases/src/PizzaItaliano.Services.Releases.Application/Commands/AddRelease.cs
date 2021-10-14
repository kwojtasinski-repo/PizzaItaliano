using Convey.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Application.Commands
{
    public class AddRelease : ICommand
    {
        public Guid ReleaseId { get; set; }
        public Guid OrderId { get; set; }
        public Guid OrderProductId { get; set; }
    }
}
