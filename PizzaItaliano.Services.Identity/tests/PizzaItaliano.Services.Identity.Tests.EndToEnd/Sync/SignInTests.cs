using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.DTO;
using PizzaItaliano.Services.Identity.Application.Exceptions;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Identity.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using PizzaItaliano.Services.Identity.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.EndToEnd.Sync
{
    [Collection("Collection")]
    public class SignInTests
    {
        private Task<HttpResponseMessage> Act(SignIn command)
           => _httpClient.PostAsync("sign-in", TestHelper.GetContent(command));

        [Fact]
        public async Task sign_in_endpoint_should_return_token()
        {
            var command = new SignIn("emailabc@email.com", "PAsW0RDd13!2");
            await _identityService.SignUpAsync(new SignUp(Guid.Empty, command.Email, command.Password, "user", Enumerable.Empty<string>()));

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var auth = await response.Content.ReadFromJsonAsync<AuthDto>();
            auth.ShouldNotBeNull();
            auth.AccessToken.ShouldNotBeNull();
            auth.AccessToken.Length.ShouldBeGreaterThan(1);
        }

        [Fact]
        public async Task sign_in_endpoint_with_invalid_password_should_return_http_status_code_bad_request()
        {
            var command = new SignIn("emailabc12378@email.com", "PAsW0RDd13!2");
            await _identityService.SignUpAsync(new SignUp(Guid.Empty, command.Email, command.Password + "a24", "user", Enumerable.Empty<string>()));

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new InvalidCredentialsException(command.Email);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task sign_in_endpoint_with_not_existing_user_should_return_http_status_code_bad_request()
        {
            var command = new SignIn("emailabc12213543@email.com", "PAsW0RDd13!2");

            await Act(command);
            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new InvalidCredentialsException(command.Email);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixture;
        private readonly IIdentityService _identityService;

        public SignInTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("users");
            _httpClient = factory.CreateClient();
            factory.Server.AllowSynchronousIO = true;
            _identityService = factory.Services.GetRequiredService<IIdentityService>();
        }

        #endregion
    }
}
