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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Products.Tests.Intgration.Async
{
    public class UpdateProductTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task Act(UpdateProduct command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task update_product_command_should_update_document_with_given_id()
        {
            var productId = Guid.NewGuid();
            await TestHelper.AddTestProduct(productId, _mongoDbFixture);
            var command = new UpdateProduct(productId, "Pizza Capriciosa", new decimal(35.52));

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<ProductModified, ProductDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.ProductId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.ProductId);
            document.Name.ShouldBe(command.Name);
            document.Cost.ShouldBe(command.Cost.Value);
        }

        [Fact]
        public async Task update_product_command_with_invalid_name_and_cost_should_throw_an_exception_and_send_rejected_event()
        {
            var productId = Guid.NewGuid();
            await TestHelper.AddTestProduct(productId, _mongoDbFixture);
            var command = new UpdateProduct(productId, "", null);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<UpdateProductRejected>(Exchange);

            await Act(command);

            var updateOrderRejected = await tcs.Task;

            updateOrderRejected.ShouldNotBeNull();
            updateOrderRejected.ShouldBeOfType<UpdateProductRejected>();
            var exception = new InvalidUpdateProductException(productId);
            updateOrderRejected.Code.ShouldBe(exception.Code);
            updateOrderRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task update_product_command_with_invalid_cost_should_throw_an_exception_and_send_rejected_event()
        {
            var productId = Guid.NewGuid();
            await TestHelper.AddTestProduct(productId, _mongoDbFixture);
            var command = new UpdateProduct(productId, "", new decimal(-200));

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<UpdateProductRejected>(Exchange);

            await Act(command);

            var updateOrderRejected = await tcs.Task;

            updateOrderRejected.ShouldNotBeNull();
            updateOrderRejected.ShouldBeOfType<UpdateProductRejected>();
            var exception = new InvalidProductCostException(productId);
            updateOrderRejected.Code.ShouldBe(exception.Code);
            updateOrderRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task update_product_command_with_invalid_id_should_throw_an_exception_and_send_rejected_event()
        {
            var productId = Guid.NewGuid();
            var command = new UpdateProduct(productId, "abc", new decimal(100));

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<UpdateProductRejected>(Exchange);

            await Act(command);

            var updateOrderRejected = await tcs.Task;

            updateOrderRejected.ShouldNotBeNull();
            updateOrderRejected.ShouldBeOfType<UpdateProductRejected>();
            var exception = new ProductNotFoundException(productId);
            updateOrderRejected.Code.ShouldBe(exception.Code);
            updateOrderRejected.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private const string Exchange = "product";
        private readonly MongoDbFixture<ProductDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public UpdateProductTests(PizzaItalianoApplicationFactory<Program> factory)
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
