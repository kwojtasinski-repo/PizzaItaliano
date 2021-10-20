using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using System;

namespace PizzaItaliano.Services.Releases.Application
{
    public static class Extensions
    {
        public static IConveyBuilder AddApplication(this IConveyBuilder builder)
        {
            builder.AddCommandHandlers()
                   .AddEventHandlers()
                   .AddInMemoryCommandDispatcher()
                   .AddInMemoryEventDispatcher();

            return builder;
        }
    }
}
