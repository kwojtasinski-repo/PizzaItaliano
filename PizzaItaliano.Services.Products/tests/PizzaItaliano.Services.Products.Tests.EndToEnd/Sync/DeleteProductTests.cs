using PizzaItaliano.Services.Products.API;
using PizzaItaliano.Services.Products.Application.Commands;
using PizzaItaliano.Services.Products.Application.Exceptions;
using PizzaItaliano.Services.Products.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Products.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Products.Tests.Shared.Factories;
using PizzaItaliano.Services.Products.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Products.Tests.EndToEnd.Sync
{
    public class DeleteProductTests : IDisposable, IClassFixture<PizzaItalianoApplicationFactory<Program>>
    {
        private Task<HttpResponseMessage> Act(DeleteProduct command)
            => _httpClient.DeleteAsync($"products/{command.ProductId}");

        [Fact]
        public async Task delete_product_endpoint_should_return_http_status_code_created()
        {
            var productId = Guid.NewGuid();
            await TestHelper.AddTestProduct(productId, _mongoDbFixture);
            var command = new DeleteProduct(productId);

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task delete_product_endpoint_should_return_body_with_correct_order_id()
        {
            var productId = Guid.NewGuid();
            await TestHelper.AddTestProduct(productId, _mongoDbFixture);
            var command = new DeleteProduct(productId);
            var expectedResponse = $"Product with {productId} deleted";

            var response = await Act(command);
            var bodyString = TestHelper.MapTo<string>(await response.Content.ReadAsStringAsync());

            bodyString.ShouldNotBeNull();
            bodyString.ShouldBe(expectedResponse);
        }

        [Fact]
        public async Task delete_product_endpoint_should_delete_document_with_given_id_from_database()
        {
            var productId = Guid.NewGuid();
            await TestHelper.AddTestProduct(productId, _mongoDbFixture);
            var command = new DeleteProduct(productId);

            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.ProductId);

            document.ShouldBeNull();
        }

        [Fact]
        public async Task delete_product_endpoint_with_invalid_id_should_throw_an_exception_and_send_bad_request()
        {
            var productId = Guid.NewGuid();
            var command = new DeleteProduct(productId);

            var response = await Act(command);
            var error = TestHelper.MapTo<TestHelper.Error>(await response.Content.ReadAsStringAsync());

            error.ShouldNotBeNull();
            error.ShouldBeOfType<TestHelper.Error>();
            var exception = new ProductNotFoundException(productId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task delete_product_command_with_invalid_status_should_throw_an_exception_and_send_bad_request()
        {
            var productId = Guid.NewGuid();
            await TestHelper.AddTestProduct(productId, Core.Entities.ProductStatus.Used, _mongoDbFixture);
            var command = new DeleteProduct(productId);

            var response = await Act(command);
            var error = TestHelper.MapTo<TestHelper.Error>(await response.Content.ReadAsStringAsync());

            error.ShouldNotBeNull();
            error.ShouldBeOfType<TestHelper.Error>();
            var exception = new CannotDeleteProductException(productId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<ProductDocument, Guid> _mongoDbFixture;

        public DeleteProductTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<ProductDocument, Guid>("products");
            factory.Server.AllowSynchronousIO = true;
            _httpClient = factory.CreateClient();
        }

        public void Dispose()
        {
            _mongoDbFixture.Dispose();
        }

        #endregion
    }
}
