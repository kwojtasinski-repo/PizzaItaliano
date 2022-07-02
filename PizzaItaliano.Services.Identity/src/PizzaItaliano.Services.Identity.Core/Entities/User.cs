using PizzaItaliano.Services.Identity.Core.Entities.ValueObjects;
using PizzaItaliano.Services.Identity.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PizzaItaliano.Services.Identity.Core.Entities
{
    public class User : AggregateRoot
    {
        public Email Email { get; private set; }
        public string Role { get; private set; }
        public string Password { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public IEnumerable<string> Permissions { get; private set; }

        public User(Guid id, string email, string password, string role, DateTime createdAt,
            IEnumerable<string> permissions = null)
        {
            if (!Entities.Role.IsValid(role))
            {
                throw new InvalidRoleException(role);
            }

            Id = id;
            Email = Email.From(email);
            Password = password;
            Role = role.ToLowerInvariant();
            CreatedAt = createdAt;
            Permissions = permissions ?? Enumerable.Empty<string>();
        }
    }
}
