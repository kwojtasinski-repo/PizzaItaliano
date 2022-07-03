using PizzaItaliano.Services.Identity.Core.Entities.ValueObjects;
using PizzaItaliano.Services.Identity.Core.Exceptions;
using Shouldly;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.Unit.Core.Entities.ValueObjects
{
    public class PasswordTests
    {
        private Password Act(string password) => Password.From(password);

        [Fact]
        public void should_create_password()
        {
            var password = "PAsSW0ORd1!23";

            var act = Act(password);

            act.ShouldNotBeNull();
            act.Value.ShouldBe(password);
        }

        [Fact]
        public void given_null_password_should_throw_an_exception()
        {
            var expectedException = new InvalidPasswordException();

            var exception = Record.Exception(() => Act(null));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public void given_invalid_password_should_throw_an_exception()
        {
            var password = "password";
            var expectedException = new InvalidPasswordException();

            var exception = Record.Exception(() => Act(password));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }
    }
}
