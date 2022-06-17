using Newtonsoft.Json;
using PizzaItaliano.Services.Releases.API;
using PizzaItaliano.Services.Releases.Application.Commands;
using PizzaItaliano.Services.Releases.Application.Exceptions;
using PizzaItaliano.Services.Releases.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Releases.Tests.Shared.Factories;
using PizzaItaliano.Services.Releases.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Releases.Tests.EndToEnd
{
    [Collection("Collection")]
    public class AddReleaseTests
    {
        private Task<HttpResponseMessage> Act(AddRelease command)
            => _httpClient.PostAsync("releases", GetContent(command));

        [Fact]
        public async Task add_release_endpoint_should_return_http_status_code_created()
        {
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new AddRelease() { ReleaseId = releaseId, OrderId = orderId, OrderProductId = orderProductId };

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task add_release_endpoint_should_return_location_header_with_correct_order_id()
        {
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new AddRelease() { ReleaseId = releaseId, OrderId = orderId, OrderProductId = orderProductId };

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();

            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"releases/{command.ReleaseId}");
        }

        [Fact]
        public async Task add_release_endpoint_should_add_document_with_given_id_to_database()
        {
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            var command = new AddRelease() { ReleaseId = releaseId, OrderId = orderId, OrderProductId = orderProductId };

            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.ReleaseId);

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.ReleaseId);
            document.OrderId.ShouldBe(command.OrderId);
            document.OrderProductId.ShouldBe(command.OrderProductId);
        }

        [Fact]
        public async Task add_release_endpoint_with_invalid_id_should_return_bad_request()
        {
            var releaseId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var orderProductId = Guid.NewGuid();
            await _mongoDbFixture.InsertAsync(new ReleaseDocument() { Id = releaseId, OrderId = orderId, OrderProductId = orderProductId, Date = DateTime.Now });
            var command = new AddRelease() { ReleaseId = releaseId, OrderId = orderId, OrderProductId = orderProductId };

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = MapTo<Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new ReleaseAlreadyExistsException(releaseId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        private static StringContent GetContent(object value)
            => new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

        private static T MapTo<T>(string stringResponse) 
            => JsonConvert.DeserializeObject<T>(stringResponse);

        class Error
        {
            public string Code { get; set; }
            public string Reason { get; set; }
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<ReleaseDocument, Guid> _mongoDbFixture;

        public AddReleaseTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<ReleaseDocument, Guid>("releases");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        #endregion
    }
}
