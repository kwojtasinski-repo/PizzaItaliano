using PizzaItaliano.Services.Orders.Application;

namespace PizzaItaliano.Services.Orders.Infrastructure.Contexts
{
    public interface IAppContextFactory
    {
        IAppContext Create();
    }
}