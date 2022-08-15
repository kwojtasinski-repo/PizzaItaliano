using Convey.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PizzaItaliano.Services.Identity.Tests.Shared.Helpers
{
    public class TestUsers
    {
        private const string adminUser = "admin@admin-pizza.com";
        private static IList<UserDocument> _users = new List<UserDocument>
        {
            new UserDocument { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Email = "123532465346@gaw.com", Password = "test", Role = "admin" },
            new UserDocument { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Email = "154673@abc.com", Password = "test", Role = "user" },
            new UserDocument { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Email = "123572@q1a.com", Password = "test", Role = "user" },
            new UserDocument { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Email = "64578@avs.com", Password = "test", Role = "admin" },
            new UserDocument { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Email = "23573235@sed.com", Password = "test", Role = "user" },
            new UserDocument { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Email = adminUser, Password = "admin", Role = "admin" },
        };

        public static IList<UserDocument> GetTestUsers()
        {
            return _users;
        }

        public static UserDocument GetAdmin()
        {
            return _users.Where(u => u.Email == adminUser).Single();
        }

        public sealed class UserDocument : IIdentifiable<Guid>
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Password { get; set; }
            public DateTime CreatedAt { get; set; }
            public IEnumerable<string> Permissions { get; set; }

            public UserDocument()
            {
            }

            public UserDocument(UserDocument userDocument)
            {
                Id = userDocument.Id;
                Email = userDocument.Email;
                Role = userDocument.Role;
                Password = userDocument.Password;
                CreatedAt = userDocument.CreatedAt;
                Permissions = userDocument.Permissions;
            }
        }
    }
}
