using System;
using System.Collections.Generic;

namespace PizzaItaliano.Services.Releases.Tests.Shared.Fixtures
{
    public class UserFixture
    {
        public static User CreateSampleUser()
        {
            var user = new User(Guid.NewGuid(), "admin");
            return user;
        }

        public class User
        {
            public Guid Id { get; }
            public string Role { get; } = string.Empty;
            public bool IsAuthenticated { get; }
            public bool IsAdmin { get; }
            public IDictionary<string, string> Claims { get; } = new Dictionary<string, string>();

            public User(Guid id, string role, IDictionary<string, string> claims = null)
            {
                Id = id;
                Role = role;
                IsAuthenticated = Id != Guid.Empty;
                IsAdmin = role.ToLowerInvariant() == "admin";

                if (claims is not null)
                {
                    Claims = claims;
                }
            }
        }
    }
}
