using PizzaItaliano.Services.Payments.Core.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Core.Entities
{
    public abstract class AggregateRoot
    {
        private readonly IList<IDomainEvent> _events = new List<IDomainEvent>();
        public IEnumerable<IDomainEvent> Events => _events;
        [TypeConverter(typeof(IdGuidConverterCustom))] // konwersja runtime refleksja
        public AggregateId Id { get; protected set; }
        public int Version { get; protected set; }

        protected void AddEvent(IDomainEvent @event)
        {
            _events.Add(@event);
        }

        public void ClearEvents() => _events.Clear();
    }
}
