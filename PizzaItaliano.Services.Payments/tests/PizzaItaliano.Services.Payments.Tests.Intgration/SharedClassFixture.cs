using PizzaItaliano.Services.Payments.Tests.Intgration.Helpers;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.Intgration
{
    [CollectionDefinition("Collection")]
    public class SharedClassFixture : ICollectionFixture<TestAppFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
