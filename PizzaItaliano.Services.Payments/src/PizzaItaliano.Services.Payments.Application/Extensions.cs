using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PizzaItaliano.Services.Payments.Tests.Unit")] // widocznosc internal na poziomie testow (integration)
namespace PizzaItaliano.Services.Payments.Application
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
