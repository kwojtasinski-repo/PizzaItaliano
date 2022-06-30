using PizzaItaliano.Services.Identity.Core.Entities;
using PizzaItaliano.Services.Identity.Core.Exceptions;
using Shouldly;
using System;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.Unit.Entities
{
    public class UserTests
    {
        [Fact]
        public void given_empty_email_should_throw_an_exception()
        {
            var id = Guid.NewGuid();
            var email = "";
            var password = "password@A1!";
            var role = Role.User;
            var createdAt = DateTime.UtcNow;
            var expectedException = new InvalidEmailException(email);

            var exception = Record.Exception(() => new User(id, email, password, role, createdAt));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
        }

        [Fact]
        public void given_empty_password_should_throw_an_exception()
        {
            var id = Guid.NewGuid();
            var email = "test@test.com";
            var password = "";
            var role = Role.User;
            var createdAt = DateTime.UtcNow;
            var expectedException = new InvalidPasswordException();

            var exception = Record.Exception(() => new User(id, email, password, role, createdAt));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
        }

        [Fact]
        public void given_invalid_role_should_throw_an_exception()
        {
            var id = Guid.NewGuid();
            var email = "test@test.com";
            var password = "password@A1!";
            var role = "@Abc";
            var createdAt = DateTime.UtcNow;
            var expectedException = new InvalidRoleException(role);

            var exception = Record.Exception(() => new User(id, email, password, role, createdAt));

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType(expectedException.GetType());
        }

        [Fact]
        public void should_create_user()
        {
            var id = Guid.NewGuid();
            var email = "test@test.com";
            var password = "password@A1!";
            var role = Role.User;
            var createdAt = DateTime.UtcNow;

            var user = new User(id, email, password, role, createdAt);

            user.ShouldNotBeNull();
            user.Email.ShouldBe(email);
            user.Password.ShouldBe(password);
            user.Role.ShouldBe(role);
            user.CreatedAt.ShouldBe(createdAt);
        }
    }
}
