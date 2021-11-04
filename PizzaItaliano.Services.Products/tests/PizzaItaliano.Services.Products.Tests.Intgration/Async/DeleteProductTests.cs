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
    public class DeleteProductTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task Act(DeleteProduct command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task delete_product_command_should_delete_document_with_given_id_from_database()
        {
            var productId = Guid.NewGuid();
            await TestHelper.AddTestProduct(productId, _mongoDbFixture);
            var command = new DeleteProduct(productId);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<ProductDeleted, ProductDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.ProductId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldBeNull();
        }

        [Fact]
        public async Task delete_product_command_with_invalid_id_should_throw_an_exception_and_send_rejected_event()
        {
            var productId = Guid.NewGuid();
            var command = new DeleteProduct(productId);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<DeleteProductRejected>(Exchange);

            await Act(command);

            var deleteProductRejected = await tcs.Task;

            deleteProductRejected.ShouldNotBeNull();
            deleteProductRejected.ShouldBeOfType<DeleteProductRejected>();
            var exception = new ProductNotFoundException(productId);
            deleteProductRejected.Code.ShouldBe(exception.Code);
            deleteProductRejected.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task delete_product_command_with_invalid_status_should_throw_an_exception_and_send_rejected_event()
        {
            var productId = Guid.NewGuid();
            await TestHelper.AddTestProduct(productId, Core.Entities.ProductStatus.Used, _mongoDbFixture);
            var command = new DeleteProduct(productId);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<DeleteProductRejected>(Exchange);

            await Act(command);

            var deleteProductRejected = await tcs.Task;

            deleteProductRejected.ShouldNotBeNull();
            deleteProductRejected.ShouldBeOfType<DeleteProductRejected>();
            var exception = new CannotDeleteProductException(productId);
            deleteProductRejected.Code.ShouldBe(exception.Code);
            deleteProductRejected.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private const string Exchange = "product";
        private readonly MongoDbFixture<ProductDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public DeleteProductTests(PizzaItalianoApplicationFactory<Program> factory)
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
