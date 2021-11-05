using Convey;
using Convey.Logging.CQRS;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Releases.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Infrastructure.Logging
{
    internal static class Extensions
    {
        public static IConveyBuilder AddHandlersLogging(this IConveyBuilder builder)
        {
            var assembly = typeof(AddRelease).Assembly;

            builder.Services.AddSingleton<IMessageToLogTemplateMapper, MessageToLogTemplateMapper>();
            builder.AddCommandHandlersLogging(assembly);
            builder.AddEventHandlersLogging(assembly);

            return builder;
        }
    }
}
