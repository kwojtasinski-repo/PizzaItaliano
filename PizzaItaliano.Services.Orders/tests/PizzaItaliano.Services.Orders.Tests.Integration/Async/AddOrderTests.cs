using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Events;
using PizzaItaliano.Services.Orders.Application.Events.Rejected;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Tests.Shared.Factories;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Integration.Async
{
    public class AddOrderTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task Act(AddOrder command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task add_order_command_should_add_document_with_given_id_to_database()
        {
            var orderId = Guid.NewGuid();
            var command = new AddOrder(orderId);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<OrderAdded, OrderDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.OrderId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.OrderId);
        }

        [Fact]
        public async Task add_order_command_with_existed_should_throw_an_exception()
        {
            var orderId = Guid.NewGuid();
            AddTestOrder(orderId);
            var command = new AddOrder(orderId);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddOrderRejected>(Exchange);

            await Act(command);

            var addOrderRejected = await tcs.Task;

            addOrderRejected.ShouldNotBeNull();
            addOrderRejected.ShouldBeOfType<AddOrderRejected>();
            var exception = new OrderAlreadyExistsException(orderId);
            addOrderRejected.Code.ShouldBe(exception.Code);
            addOrderRejected.Reason.ShouldBe(exception.Message);
        }

        private void AddTestOrder(Guid id)
        {
            var orderId = id;
            var cost = new decimal(100);
            var orderDate = DateTime.Now;
            var orderNumber = "123";
            var status = Core.Entities.OrderStatus.Released;
            var orderProductId = Guid.NewGuid();
            var orderProductStatus = Core.Entities.OrderProductStatus.Released;
            var productId = Guid.NewGuid();
            var orderProductDocument = new OrderProductDocument { Id = orderProductId, Cost = cost, OrderId = orderId, Quantity = 1, OrderProductStatus = orderProductStatus, ProductId = productId };
            var products = new List<OrderProductDocument> { orderProductDocument };
            var releasedDate = DateTime.Now;
            var document = new OrderDocument { Id = orderId, Cost = cost, OrderDate = orderDate, OrderNumber = orderNumber, OrderStatus = status, OrderProductDocuments = products, ReleaseDate = releasedDate, Version = 0 };

            _mongoDbFixture.InsertAsync(document);
        }

        #region Arrange

        private const string Exchange = "order";
        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public AddOrderTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<OrderDocument, Guid>("orders");
            var testServer = factory.GetTestServer();
            testServer.AllowSynchronousIO = true;
        }

        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}
