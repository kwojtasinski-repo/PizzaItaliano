using PizzaItaliano.Services.Payments.API;
using PizzaItaliano.Services.Payments.Tests.Shared.Factories;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.Intgration
{
    [CollectionDefinition("Collection")]
    public class SharedClassFixture : ICollectionFixture<PizzaItalianoApplicationFactory<Program>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
