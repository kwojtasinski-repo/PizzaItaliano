using PizzaItaliano.Services.Payments.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Core.Converters
{
    public class IdGuidConverterCustom : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context,
                                       System.Globalization.CultureInfo culture,
                                       object value)
        {
            if (value is Guid)
                return new AggregateId((Guid)value);
            else if (value is AggregateId)
                return value;
            return base.ConvertFrom(context, culture, value);
        }
    }
}
