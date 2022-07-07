using PizzaItaliano.Services.Identity.Application.Commands;
using PizzaItaliano.Services.Identity.Application.Events;
using PizzaItaliano.Services.Identity.Application.Events.Rejected;
using PizzaItaliano.Services.Identity.Application.Exceptions;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Identity.Tests.Integration.Helpers;
using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using PizzaItaliano.Services.Identity.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.Integration.Async
{
    [Collection("Collection")]
    public class SignUpTests
    {
        private Task Act(SignUp command) => _rabbitMqFixture.PublishAsync(command, Exchange);

        [Fact]
        public async Task sign_up_command_should_add_document_with_given_id_to_database()
        {
            var command = new SignUp(Guid.NewGuid(), "email1@email.com", "PAsW0Rd!12cd", "user", Enumerable.Empty<string>());

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<SignedUp, UserDocument>(Exchange,
                    _mongoDbFixture.GetAsync, command.UserId);

            await Act(command);

            var document = await tcs.Task;

            document.ShouldNotBeNull();
            document.Id.ShouldBe(command.UserId);
        }

        [Fact]
        public async Task sign_up_command_with_existing_email_should_throw_an_exception_and_send_rejected_event()
        {
            var command = new SignUp(Guid.NewGuid(), "email2@email.com", "PAsW0Rd!12cd", "user", Enumerable.Empty<string>());
            await TestHelper.AddTestUser(command.UserId, command.Email, command.Password, _mongoDbFixture);

            var tcs = _rabbitMqFixture
                .SubscribeAndGet<SignUpRejected>(Exchange);

            await Act(command);

            var signUpRejected = await tcs.Task;

            signUpRejected.ShouldNotBeNull();
            signUpRejected.ShouldBeOfType<SignUpRejected>();
            var exception = new EmailInUseException(command.Email);
            signUpRejected.Code.ShouldBe(exception.Code);
            signUpRejected.Reason.ShouldBe(exception.Message);
        }

        #region Arrange

        private const string Exchange = "identity";
        private readonly RabbitMqFixture _rabbitMqFixture;
        private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixture;

        public SignUpTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _rabbitMqFixture = new RabbitMqFixture();
            _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("users");
            factory.Server.AllowSynchronousIO = true;
        }

        #endregion
    }
}
