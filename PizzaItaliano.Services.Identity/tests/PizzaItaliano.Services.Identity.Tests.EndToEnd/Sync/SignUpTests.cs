using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.Exceptions;
using PizzaItaliano.Services.Identity.Core.Exceptions;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Identity.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using PizzaItaliano.Services.Identity.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.EndToEnd.Sync
{
    [Collection("Collection")]
    public class SignUpTests
    {
        private Task<HttpResponseMessage> Act(SignUp command)
            => _httpClient.PostAsync("sign-up", TestHelper.GetContent(command));

        [Fact]
        public async Task sign_up_endpoint_should_return_http_status_code_created()
        {
            var userId = Guid.NewGuid();
            var command = new SignUp(userId, "emailabc@email.com", "PAsW0RDd13!2", "user", Enumerable.Empty<string>());

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task sign_up_endpoint_should_return_location_header_with_correct_message()
        {
            var userId = Guid.NewGuid();
            var command = new SignUp(userId, "email1a@email.com", "PAsW0RDd13!2", "user", Enumerable.Empty<string>());

            var response = await Act(command);

            var locationHeader = response.Headers.FirstOrDefault(h => h.Key == "Location").Value.First();

            locationHeader.ShouldNotBeNull();
            locationHeader.ShouldBe("identity/me");
        }

        [Fact]
        public async Task sign_up_endpoint_should_add_document_with_given_id_to_database()
        {
            var userId = Guid.NewGuid();
            var command = new SignUp(userId, "emailAba@email.com", "PAsW0RDd13!2", "user", Enumerable.Empty<string>());

            await Act(command);
            var document = await _mongoDbFixture.GetAsync(command.UserId);

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.UserId);
        }

        [Fact]
        public async Task sign_up_endpoint_with_existing_email_should_return_http_status_code_bad_request()
        {
            var userId = Guid.NewGuid();
            await TestHelper.AddTestUser(userId, "email2a@email.com", "secret", _mongoDbFixture);
            var command = new SignUp(Guid.Empty, "email2a@email.com", "PAsW0RDd13!2", "user", Enumerable.Empty<string>());

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new EmailInUseException(command.Email);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }
        
        [Fact]
        public async Task sign_up_endpoint_with_invalid_email_should_return_http_status_code_bad_request()
        {
            var userId = Guid.NewGuid();
            var command = new SignUp(Guid.Empty, "email", "PAsW0RDd13!2", "user", Enumerable.Empty<string>());

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new InvalidEmailException(command.Email);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task sign_up_endpoint_with_invalid_password_should_return_http_status_code_bad_request()
        {
            var userId = Guid.NewGuid();
            var command = new SignUp(userId, "emailAtest@email.com", "secret", "user", Enumerable.Empty<string>());

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new InvalidPasswordException();
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixture;

        public SignUpTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("users");
            _httpClient = factory.CreateClient();
            factory.Server.AllowSynchronousIO = true;
        }

        #endregion
    }
}
