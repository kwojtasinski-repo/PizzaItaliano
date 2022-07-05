using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Tests.Integration.Helpers;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Integration.Sync
{
    [Collection("Collection")]
    public class AddOrderTests
    {
        private Task Act(OrderDocument document) => _mongoDbFixture.InsertAsync(document);

        [Fact]
        public async Task add_order_command_should_add_document_with_given_id_to_database()
        {
            var orderId = Guid.NewGuid();
            var cost = new decimal(100);
            var orderDate = DateTime.Now;
            var orderNumber = "123";
            var status = Core.Entities.OrderStatus.Released;
            var orderProductId = Guid.NewGuid();
            var orderProductStatus = Core.Entities.OrderProductStatus.Released;
            var productId = Guid.NewGuid();
            var orderProductDocument = new OrderProductDocument { Id = orderProductId, Cost = cost, OrderId = orderId, Quantity = 1, OrderProductStatus = orderProductStatus, ProductId = productId };
            var products = new List<OrderProductDocument> { orderProductDocument};
            var releasedDate = DateTime.Now;
            var userId = Guid.NewGuid();
            var document = new OrderDocument { Id = orderId, Cost = cost, OrderDate = orderDate, OrderNumber = orderNumber, OrderStatus = status, OrderProductDocuments = products, ReleaseDate = releasedDate, Version = 0, UserId = userId };

            await Act(document);
            var documentFromDb = await _mongoDbFixture.GetAsync(document.Id);

            documentFromDb.ShouldNotBeNull();
            documentFromDb.ShouldBeOfType<OrderDocument>();
        }

        #region Arrange

        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;

        public AddOrderTests(TestAppFactory factory)
        {
            _mongoDbFixture = new MongoDbFixture<OrderDocument, Guid>("orders");
        }

        #endregion
    }
}
