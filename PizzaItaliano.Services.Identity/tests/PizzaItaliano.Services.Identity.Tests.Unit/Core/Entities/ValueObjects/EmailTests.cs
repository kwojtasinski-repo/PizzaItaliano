using PizzaItaliano.Services.Identity.Core.Entities.ValueObjects;
using PizzaItaliano.Services.Identity.Core.Exceptions;
using Shouldly;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.Unit.Core.Entities.ValueObjects
{
PizzaItaliano.Services.Orders.Tests.Integration    public class EmailTests
    {
        private Email Act(string email) => Email.From(email);

        [Fact]
        public void should_create_email()
        {
            var email = "email@email.com";

            var result = Act(email);

            result.ShouldNotBeNull();
            result.Value.ShouldBe(email);
        }

        [Fact]
        public void given_invalid_email_should_throw_an_exception()
        {
            var email = "email@email";
            var expectedException = new InvalidEmailException(email);

            var exception = Record.Exception(() => Act(email));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }

        [Fact]
        public void given_null_email_should_throw_an_exception()
        {
            string email = null;
            var expectedException = new InvalidEmailException(email);

            var exception = Record.Exception(() => Act(email));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
            exception.Message.ShouldBe(expectedException.Message);
        }
    }
}
