using Convey.HTTP;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Payments.Application.Events;
using PizzaItaliano.Services.Payments.Application.Events.External;
using PizzaItaliano.Services.Payments.Core.Entities;
using PizzaItaliano.Services.Payments.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Payments.Tests.Intgration.Helpers;
using PizzaItaliano.Services.Payments.Tests.Shared;
using PizzaItaliano.Services.Payments.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace PizzaItaliano.Services.Payments.Tests.Intgration.Async
{
    [Collection("Collection")]
    public class OrderStateModifiedTests
    {
        private Task Act(OrderStateModified command) => _rabbitMqFixture.PublishAsync(command, "order");

        [Fact]
        public async Task should_create_payment()
        {
            var orderId = Guid.NewGuid();
            var command = new OrderStateModified(orderId, OrderStatus.New, OrderStatus.Ready);
            Expression<Func<PaymentDocument, bool>> expression = p => p.OrderId == orderId;
            _wireMockServer.Given(
              Request.Create()
                .WithPath($"/orders/{orderId}")
                .UsingGet()).RespondWith(
              Response.Create()
                .WithBody(GetOrderResponseString(orderId))
                .WithStatusCode(200));

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<AddedPayment, PaymentDocument>(Exchange,
                    _mongoDbFixture.GetAsync, expression);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.PaymentStatus.ShouldBe(PaymentStatus.Unpaid);
        }

        private static string GetOrderResponseString(Guid id)
        {
            return "{ \"id\": \"" + id + "\", \"orderNumber\": \"ORD/123\", \"cost\": 123.25 }";
        }

        #region Arrange

        private const string Exchange = "payment";
        private readonly MongoDbFixture<PaymentDocument, Guid> _mongoDbFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;
        private readonly WireMockServer _wireMockServer;
        private readonly string _url;

        public OrderStateModifiedTests(TestAppFactory factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<PaymentDocument, Guid>("payments");
            factory.Server.AllowSynchronousIO = true;
            _wireMockServer = WireMockServer.Start();
            var options = factory.Services.GetRequiredService<HttpClientOptions>();
            _url = _wireMockServer.Urls.Single();
            options.Services["orders"] = _url;
        }

        #endregion
    }
}
