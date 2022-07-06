using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.Integration
{
    [CollectionDefinition("Collection")]
    public class SharedClassFixture : ICollectionFixture<PizzaItalianoApplicationFactory<Program>>
    {
    }
}
