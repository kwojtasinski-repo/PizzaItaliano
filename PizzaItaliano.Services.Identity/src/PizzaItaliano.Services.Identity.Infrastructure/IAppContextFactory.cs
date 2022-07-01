using PizzaItaliano.Services.Identity.Application;

namespace PizzaItaliano.Services.Identity.Infrastructure
{
    public interface IAppContextFactory
    {
        IAppContext Create();
    }
}