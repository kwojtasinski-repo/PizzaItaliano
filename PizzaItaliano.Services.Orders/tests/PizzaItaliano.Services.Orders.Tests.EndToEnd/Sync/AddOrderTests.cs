using PizzaItaliano.Services.Orders.API;
using PizzaItaliano.Services.Orders.Tests.Shared.Factories;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using System;
using System.Net.Http;
using Xunit;
using System.Threading.Tasks;
using PizzaItaliano.Services.Orders.Application.Commands;
using Shouldly;
using System.Net;
using PizzaItaliano.Services.Orders.Tests.EndToEnd.Helpers;
using System.Linq;
using PizzaItaliano.Services.Orders.Application.Exceptions;

namespace PizzaItaliano.Services.Orders.Tests.EndToEnd.Sync
{

    [Collection("Collection")]
    public class AddOrderTests
    {
        private Task<HttpResponseMessage> Act(AddOrder command)
            => _httpClient.PostAsync("orders", TestHelper.GetContent(command));

        [Fact]
        public async Task add_order_endpoint_should_return_http_status_code_created()
        {
            var orderId = Guid.NewGuid();
            var command = new AddOrder(orderId);
            _httpClient.DefaultRequestHeaders.Add("Correlation-Context", "{\"user\": { \"id\": \"5ade56cd-76d4-48a5-804b-f3ba033e136d\", \"isAuthenticated\": \"true\", \"role\": \"admin\", \"claims\": {} }}");

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task add_order_endpoint_should_return_location_header_with_correct_order_id()
        {
            var orderId = Guid.NewGuid();
            var command = new AddOrder(orderId);

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();

            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"orders/{command.OrderId}");
        }

        [Fact]
        public async Task add_order_endpoint_should_add_document_with_given_id_to_database()
        {
            var orderId = Guid.NewGuid();
            var command = new AddOrder(orderId);

            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.OrderId);

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.OrderId);
        }

        [Fact]
        public async Task add_order_endpoint_should_return_http_status_code_bad_request()
        {
            var orderId = Guid.NewGuid();
            var command = new AddOrder(orderId);

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new OrderAlreadyExistsException(orderId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<OrderDocument, Guid> _mongoDbFixture;

        public AddOrderTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<OrderDocument, Guid>("orders");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        #endregion
    }
}
