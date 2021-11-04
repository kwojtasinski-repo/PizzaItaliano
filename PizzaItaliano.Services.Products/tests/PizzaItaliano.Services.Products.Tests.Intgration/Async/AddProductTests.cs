using PizzaItaliano.Services.Products.API;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Events;
using PizzaItaliano.Services.Products.Application.Events.Rejected;
using PizzaItaliano.Services.Products.Application.Exceptions;
using PizzaItaliano.Services.Products.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Products.Tests.Intgration.Helpers;
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
        public async Task add_product_command_should_add_document_with_given_id_to_database()
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

        [Fact]
        public async Task add_product_command_should_throw_an_exception_and_send_rejected_event()
        {
            var productId = Guid.NewGuid();
            await TestHelper.AddTestProduct(productId, _mongoDbFixture);
            var command = new AddProduct(productId, "Pizza Capriciosa", new decimal(35.52));

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddProductRejected>(Exchange);

            await Act(command);

            var addOrderRejected = await tcs.Task;

            addOrderRejected.ShouldNotBeNull();
            addOrderRejected.ShouldBeOfType<AddProductRejected>();
            var exception = new ProductAlreadyExistsException(productId);
            addOrderRejected.Code.ShouldBe(exception.Code);
            addOrderRejected.Reason.ShouldBe(exception.Message);
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
