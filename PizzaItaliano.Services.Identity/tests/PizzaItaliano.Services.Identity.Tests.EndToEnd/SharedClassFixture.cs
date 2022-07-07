using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.EndToEnd
{
    [CollectionDefinition("Collection")]
    public class SharedClassFixture : ICollectionFixture<PizzaItalianoApplicationFactory<Program>>
    {
    }
}
