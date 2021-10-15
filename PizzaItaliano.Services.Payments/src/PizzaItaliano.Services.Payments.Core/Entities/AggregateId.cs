using PizzaItaliano.Services.Payments.Core.Converters;
using PizzaItaliano.Services.Payments.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Core.Entities
{
    public class AggregateId : IEquatable<AggregateId>
    {
        public Guid Value { get; }

        public AggregateId() : this(Guid.NewGuid())
        {

        }

        public AggregateId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new InvalidAggregateIdException(value);
            }

            Value = value;
        }

        public bool Equals(AggregateId other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((AggregateId)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        // implicit pozwala bez jawnego kastowania w () przechodzic z jednego typu na drugi w tym przypadku z AggregateId na Guid a ponizej z Guid na AggregateId
        public static implicit operator Guid(AggregateId id)
        {
            return id.Value;
        }

        public static implicit operator AggregateId(Guid id)
        {
            return new AggregateId();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
