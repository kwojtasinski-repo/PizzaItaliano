using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;

namespace PizzaItaliano.Services.Identity.Application
{
    public static class Extensions
    {
        public static IConveyBuilder AddApplication(this IConveyBuilder builder)
        {
            builder.AddCommandHandlers();
            builder.AddEventHandlers();
            builder.AddInMemoryCommandDispatcher();
            builder.AddInMemoryEventDispatcher();
            return builder;
        }
    }
}
