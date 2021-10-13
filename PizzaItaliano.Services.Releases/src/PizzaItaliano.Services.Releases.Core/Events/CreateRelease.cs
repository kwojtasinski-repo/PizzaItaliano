using PizzaItaliano.Services.Releases.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Core.Events
{
    public class CreateRelease : IDomainEvent
    {
        public Release Release { get; }

        public CreateRelease(Release release)
        {
            Release = release;
        }
    }
}
