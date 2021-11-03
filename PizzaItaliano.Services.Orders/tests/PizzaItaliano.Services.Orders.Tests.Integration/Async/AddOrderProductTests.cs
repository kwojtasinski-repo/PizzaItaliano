using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Events;
using PizzaItaliano.Services.Orders.Application.Services.Clients;
using PizzaItaliano.Services.Orders.Core.Entities;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Tests.Integration.Helpers;
using PizzaItaliano.Services.Orders.Tests.Shared.Factories;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Integration.Async
{
    public class AddOrderProductTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task Act(AddOrderProduct command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        /*[Fact] // potrzebna referencja do projektu zewnetrznego uruchomienie 2 test server i po ip dziala ale jest juz zaleznosc dlatego rezygnuje
        public async Task add_order_product_command_should_add_document_with_given_id_to_database()
        {
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestOrder(orderId, OrderStatus.New, _mongoDbFixture);
            var orderProductId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            int quantity = 1;
            var command = new AddOrderProduct(orderId, orderProductId, productId, quantity);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<OrderProductAdded, OrderDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.OrderId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.OrderId);
        }*/

        #region Arrange

        private const string Exchange = "order";
        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public AddOrderProductTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<OrderDocument, Guid>("orders");
            factory.Server.AllowSynchronousIO = true;
        }

        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}
