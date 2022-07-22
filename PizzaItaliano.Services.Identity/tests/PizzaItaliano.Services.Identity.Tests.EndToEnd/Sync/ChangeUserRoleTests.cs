using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.Exceptions;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Core.Entities;
using PizzaItaliano.Services.Identity.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using PizzaItaliano.Services.Identity.Tests.Shared.Helpers;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.EndToEnd.Sync
{
    [Collection("Collection")]
    public class ChangeUserRoleTests
    {
        private Task<HttpResponseMessage> Act(ChangeUserRole command)
           => _httpClient.PutAsync($"users/{command.Id}/change-role", TestHelper.GetContent(command));

        [Fact]
        public async Task change_user_role_endpoint_should_return_ok()
        {
            var user = _testUsers.FirstOrDefault();
            var command = new ChangeUserRole(user.Id, Role.User);
            var signUp = new SignUp(Guid.NewGuid(), "emailabafbcASD1243@email.com", "PAsW0RDd13!2", "admin", Enumerable.Empty<string>());
            await _identityService.SignUpAsync(signUp);
            var admin = await _identityService.SignInAsync(new SignIn(signUp.Email, signUp.Password));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", admin.AccessToken);

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var userChanged = await _identityService.GetAsync(user.Id);
            userChanged.Role.ShouldBe(command.Role);
        }

        [Fact]
        public async Task change_user_role_endpoint_with_invalid_user_should_return_http_status_code_bad_request()
        {
            var command = new ChangeUserRole(Guid.NewGuid(), Role.User);
            var signUp = new SignUp(Guid.NewGuid(), "emailabafbcASasd12D1243@email.com", "PAsW0RDd13!2", "admin", Enumerable.Empty<string>());
            await _identityService.SignUpAsync(signUp);
            var admin = await _identityService.SignInAsync(new SignIn(signUp.Email, signUp.Password));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", admin.AccessToken);

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new UserNotFoundException(command.Id);
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private readonly HttpClient _httpClient;
        private readonly IIdentityService _identityService;
        private readonly IList<TestUsers.UserDocument> _testUsers;

        public ChangeUserRoleTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();
            factory.Server.AllowSynchronousIO = true;
            _identityService = factory.Services.GetRequiredService<IIdentityService>();
            _testUsers = TestUsers.GetTestUsers();
        }

        #endregion
    }
}
