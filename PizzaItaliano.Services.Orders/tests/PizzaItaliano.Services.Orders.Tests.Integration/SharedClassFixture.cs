using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Tests.Shared.Factories;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Integration
{
    [CollectionDefinition("Collection")]
    public class SharedClassFixture : ICollectionFixture<PizzaItalianoApplicationFactory<Program>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
