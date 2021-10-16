using Convey;
using Convey.CQRS.Commands;

namespace PizzaItaliano.Services.Orders.Application
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
