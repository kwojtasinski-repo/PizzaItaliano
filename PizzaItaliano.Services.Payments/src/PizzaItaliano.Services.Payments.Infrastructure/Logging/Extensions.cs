using Convey;
using Convey.Logging.CQRS;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Payments.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Logging
{
    internal static class Extensions
    {
        public static IConveyBuilder AddHandlersLogging(this IConveyBuilder builder)
        {
            var assembly = typeof(AddPayment).Assembly;

            builder.Services.AddSingleton<IMessageToLogTemplateMapper, MessageToLogTemplateMapper>();
            builder.AddCommandHandlersLogging(assembly);
            builder.AddEventHandlersLogging(assembly);

            return builder;
        }
    }
}
