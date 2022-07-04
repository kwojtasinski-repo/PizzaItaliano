using PizzaItaliano.Services.Releases.Application;

namespace PizzaItaliano.Services.Releases.Infrastructure
{
    public interface IAppContextFactory
    {
        IAppContext Create();
    }
}