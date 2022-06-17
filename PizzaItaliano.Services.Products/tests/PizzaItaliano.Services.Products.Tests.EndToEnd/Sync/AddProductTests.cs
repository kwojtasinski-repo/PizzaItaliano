using PizzaItaliano.Services.Products.API;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Exceptions;
using PizzaItaliano.Services.Products.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Products.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Products.Tests.Shared.Factories;
using PizzaItaliano.Services.Products.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Products.Tests.EndToEnd
{
    [Collection("Collection")]
    public class AddProductTests
    {
        private Task<HttpResponseMessage> Act(AddProduct command)
            => _httpClient.PostAsync("products", TestHelper.GetContent(command));

        [Fact]
        public async Task add_product_endpoint_should_return_http_status_code_created()
        {
            var productId = Guid.NewGuid();
            var cost = new decimal(100);
            var name = "product";
            var command = new AddProduct(productId, name, cost);

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task add_product_endpoint_should_return_location_header_with_correct_order_id()
        {
            var productId = Guid.NewGuid();
            var cost = new decimal(100);
            var name = "product";
            var command = new AddProduct(productId, name, cost);

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();

            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe($"products/{command.ProductId}");
        }

        [Fact]
        public async Task add_product_endpoint_should_add_document_with_given_id_to_database()
        {
            var productId = Guid.NewGuid();
            var command = new AddProduct(productId, "Pizza Capriciosa", new decimal(35.52));

            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.ProductId);

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.ProductId);
        }

        [Fact]
        public async Task add_product_endpoint_should_return_http_status_code_bad_request()
        {
            var productId = Guid.NewGuid();
            var command = new AddProduct(productId, "Pizza Capriciosa", new decimal(35.52));

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new ProductAlreadyExistsException(productId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<ProductDocument, Guid> _mongoDbFixture;

        public AddProductTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<ProductDocument, Guid>("products");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        #endregion
    }
}
