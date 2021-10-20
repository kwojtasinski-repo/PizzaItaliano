using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;

namespace PizzaItaliano.Services.Orders.Application
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
