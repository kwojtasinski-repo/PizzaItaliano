using PizzaItaliano.Services.Products.API;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Events;
using PizzaItaliano.Services.Products.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Products.Tests.Shared.Factories;
using PizzaItaliano.Services.Products.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Products.Tests.Intgration.Async
{
    public class AddProductTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task Act(AddProduct command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task add_product_command_should_add_document_with_given_id_to_databaseAsync()
        {
            var command = new AddProduct(Guid.NewGuid(), "Pizza Capriciosa", new decimal(35.52));

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<ProductCreated, ProductDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.ProductId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.ProductId);
            document.Name.ShouldBe(command.Name);
            document.Cost.ShouldBe(command.Cost);
        }

        #region Arrange

        private const string Exchange = "product";
        private readonly MongoDbFixture<ProductDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public AddProductTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<ProductDocument, Guid>("products");
            factory.Server.AllowSynchronousIO = true;
        }

        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}
