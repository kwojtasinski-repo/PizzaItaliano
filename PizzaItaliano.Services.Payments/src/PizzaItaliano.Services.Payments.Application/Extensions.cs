using Convey;
using Convey.CQRS.Commands;
using System;

namespace PizzaItaliano.Services.Payments.Application
{
    public static class Extensions
    {
        public static IConveyBuilder AddApplication(this IConveyBuilder builder)
        {
            builder.AddCommandHandlers()
                .AddInMemoryCommandDispatcher();

            return builder;
        }
    }
}
