using PizzaItaliano.Services.Payments.Application;

namespace PizzaItaliano.Services.Payments.Infrastructure
{
    public interface IAppContextFactory
    {
        IAppContext Create();
    }
}