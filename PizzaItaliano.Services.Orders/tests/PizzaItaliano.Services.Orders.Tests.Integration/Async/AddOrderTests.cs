using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.Events;
using PizzaItaliano.Services.Orders.Application.Events.Rejected;
using PizzaItaliano.Services.Orders.Application.Exceptions;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Tests.Integration.Helpers;
using PizzaItaliano.Services.Orders.Tests.Shared.Factories;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Orders.Tests.Integration.Async
{
    [Collection("Collection")]
    public class AddOrderTests
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
        public async Task add_order_command_with_existed_should_throw_an_exception_and_send_rejected_event()
        {
            var orderId = Guid.NewGuid();
            await TestHelper.AddTestOrder(orderId, _mongoDbFixture);
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

        #endregion
    }
}
