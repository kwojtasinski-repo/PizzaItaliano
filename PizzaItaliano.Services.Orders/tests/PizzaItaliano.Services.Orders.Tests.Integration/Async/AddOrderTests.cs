using Microsoft.AspNetCore.TestHost;
using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Events;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Tests.Shared.Factories;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using Shouldly;
using System;
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

        #region Arrange

        private const string Exchange = "order";
        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public AddOrderTests(PizzaItalianoApplicationFactory<Program> factory)
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
