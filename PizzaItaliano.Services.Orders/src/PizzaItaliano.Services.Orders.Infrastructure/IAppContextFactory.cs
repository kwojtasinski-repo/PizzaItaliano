using PizzaItaliano.Services.Orders.Application;

namespace PizzaItaliano.Services.Orders.Infrastructure
{
    public interface IAppContextFactory
    {
        IAppContext Create();
    }
}