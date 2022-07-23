using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.Exceptions;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Core.Exceptions;
using PizzaItaliano.Services.Identity.Tests.EndToEnd.Helpers;
using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
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
    public class ChangePasswordTests
    {
        private Task<HttpResponseMessage> Act(ChangePassword command)
              => _httpClient.PostAsync("change-password", TestHelper.GetContent(command));

        [Fact]
        public async Task change_password_role_endpoint_should_return_ok()
        {
            var signUp = new SignUp(Guid.NewGuid(), "as1441fgs1rsaf@email.com", "PAsW0RDd13!2", "admin", Enumerable.Empty<string>());
            var command = new ChangePassword(signUp.Email, signUp.Password, signUp.Password + "a1234ba", signUp.Password + "a1234ba");
            await _identityService.SignUpAsync(signUp);
            var user = await _identityService.SignInAsync(new SignIn(signUp.Email, signUp.Password));
            _httpClient.AddBearerTokenToHeader(user.AccessToken);

            var response = await Act(command);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var userAfterChangePassword = await _identityService.SignInAsync(new SignIn(command.Email, command.NewPasswordConfirm));
            userAfterChangePassword.ShouldNotBeNull();
            userAfterChangePassword.AccessToken.ShouldNotBeNull();
            userAfterChangePassword.AccessToken.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task change_passowrd_role_endpoint_with_invalid_user_should_return_http_status_code_bad_request()
        {
            var signUp = new SignUp(Guid.NewGuid(), "as1441fgs1rsa56f@email.com", "PAsW0RDd13!2", "admin", Enumerable.Empty<string>());
            var command = new ChangePassword("email@emil.1234521.afasf", signUp.Password, signUp.Password + "a1234ba", signUp.Password + "a1234ba");
            await _identityService.SignUpAsync(signUp);
            var user = await _identityService.SignInAsync(new SignIn(signUp.Email, signUp.Password));
            _httpClient.AddBearerTokenToHeader(user.AccessToken);

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
        public async Task change_passowrd_role_endpoint_with_invalid_user_email_should_return_http_status_code_bad_request()
        {
            var signUp = new SignUp(Guid.NewGuid(), "cs1441fgs1rsaf@email.com", "PAsW0RDd13!2", "admin", Enumerable.Empty<string>());
            var command = new ChangePassword("email", signUp.Password, signUp.Password + "a1234ba", signUp.Password + "a1234ba");
            await _identityService.SignUpAsync(signUp);
            var user = await _identityService.SignInAsync(new SignIn(signUp.Email, signUp.Password));
            _httpClient.AddBearerTokenToHeader(user.AccessToken);

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
        public async Task change_passowrd_role_endpoint_with_not_same_passwords_should_return_http_status_code_bad_request()
        {
            var signUp = new SignUp(Guid.NewGuid(), "as1441fgs15afrsaf@email.com", "PAsW0RDd13!2", "admin", Enumerable.Empty<string>());
            var command = new ChangePassword("email", signUp.Password, signUp.Password + "a1234ba", signUp.Password + "a1234ba124");
            await _identityService.SignUpAsync(signUp);
            var admin = await _identityService.SignInAsync(new SignIn(signUp.Email, signUp.Password));
            _httpClient.AddBearerTokenToHeader(admin.AccessToken);

            var response = await Act(command);

            var bodyString = await response.Content.ReadAsStringAsync();
            var error = TestHelper.MapTo<TestHelper.Error>(bodyString);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var exception = new PasswordsAreNotSameException();
            error.Code.ShouldBe(exception.Code);
            error.Reason.ShouldBe(exception.Message);
        }

        [Fact]
        public async Task change_passowrd_role_endpoint_with_invalid_new_password_should_return_http_status_code_bad_request()
        {
            var signUp = new SignUp(Guid.NewGuid(), "as1441fgs1r21saf@email.com", "PAsW0RDd13!2", "admin", Enumerable.Empty<string>());
            var command = new ChangePassword("email", signUp.Password, "asd12", "asd12");
            await _identityService.SignUpAsync(signUp);
            var user = await _identityService.SignInAsync(new SignIn(signUp.Email, signUp.Password));
            _httpClient.AddBearerTokenToHeader(user.AccessToken);

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
        private readonly IIdentityService _identityService;

        public ChangePasswordTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();
            factory.Server.AllowSynchronousIO = true;
            _identityService = factory.Services.GetRequiredService<IIdentityService>();
        }

        #endregion
    }
}
