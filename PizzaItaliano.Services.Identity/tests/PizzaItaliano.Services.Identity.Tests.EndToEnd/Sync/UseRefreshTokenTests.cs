using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.DTO;
using PizzaItaliano.Services.Identity.Application.Exceptions;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Identity.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using PizzaItaliano.Services.Identity.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.EndToEnd.Sync
{
    [Collection("Collection")]
    public class UseRefreshTokenTests
    {
        private Task<HttpResponseMessage> Act(UseRefreshToken command)
           => _httpClient.PostAsync("refresh-tokens/use", TestHelper.GetContent(command));

        [Fact]
        public async Task should_refresh_token()
        {
            var refreshToken = "#Token";
            var userId = Guid.NewGuid();
            await TestHelper.AddTestRefreshToken(userId, refreshToken, _mongoDbFixture);
            await TestHelper.AddTestUser(userId, _mongoDbFixtureUser);
            var command = new UseRefreshToken(refreshToken);

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var auth = await response.Content.ReadFromJsonAsync<AuthDto>();
            auth.ShouldNotBeNull();
            auth.AccessToken.ShouldNotBeNull();
            auth.RefreshToken.ShouldBe(refreshToken);
        }

        [Fact]
        public async Task refresh_token_endpoint_with_not_exsiting_user_should_return_http_status_code_bad_request()
        {
            var refreshToken = "#Token12";
            var userId = Guid.NewGuid();
            await TestHelper.AddTestRefreshToken(userId, refreshToken, _mongoDbFixture);
            var command = new UseRefreshToken(refreshToken);

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new UserNotFoundException(userId);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task refresh_token_endpoint_with_revoked_token_should_return_http_status_code_bad_request()
        {
            var refreshToken = "#Token123";
            var userId = Guid.NewGuid();
            var token = await TestHelper.AddTestRefreshToken(userId, refreshToken, _mongoDbFixture);
            token.RevokedAt = DateTime.Now;
            await _mongoDbFixture.UpdateAsync(token);
            var command = new UseRefreshToken(refreshToken);

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new RevokedRefreshTokenException();
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task refresh_token_endpoint_with_invalid_token_should_return_http_status_code_bad_request()
        {
            var refreshToken = "#Token1234";
            var command = new UseRefreshToken(refreshToken);

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new InvalidRefreshTokenException();
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly MongoDbFixture<RefreshTokenDocument, Guid> _mongoDbFixture;
        private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixtureUser;

        public UseRefreshTokenTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<RefreshTokenDocument, Guid>("refreshTokens");
            _mongoDbFixtureUser = new MongoDbFixture<UserDocument, Guid>("users");
            _httpClient = factory.CreateClient();
            factory.Server.AllowSynchronousIO = true;
        }

        #endregion
    }
}
