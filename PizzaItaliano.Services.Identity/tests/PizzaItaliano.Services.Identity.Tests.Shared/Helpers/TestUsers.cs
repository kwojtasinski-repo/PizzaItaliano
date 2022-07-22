using Convey.Types;
using System;
using System.Collections.Generic;

namespace PizzaItaliano.Services.Identity.Tests.Shared.Helpers
{
    public class TestUsers
    {
        public static IList<UserDocument> GetTestUsers()
        {
            return new List<UserDocument>
            {
                new UserDocument { Id = new Guid("407716d5-2b96-4017-acdc-89b834f85e8f"), CreatedAt = DateTime.UtcNow, Email = "123532465346@gaw.com", Password = "test", Role = "admin" },
                new UserDocument { Id = new Guid("49620ea7-080b-419d-89be-ec1d855a554e"), CreatedAt = DateTime.UtcNow, Email = "154673@abc.com", Password = "test", Role = "user" },
                new UserDocument { Id = new Guid("9edacd15-0cd3-4ef0-bf03-7360b5845fb0"), CreatedAt = DateTime.UtcNow, Email = "123572@q1a.com", Password = "test", Role = "user" },
                new UserDocument { Id = new Guid("2a148762-e0c9-4eb3-8c4e-2cd6e2262cc4"), CreatedAt = DateTime.UtcNow, Email = "64578@avs.com", Password = "test", Role = "admin" },
                new UserDocument { Id = new Guid("a851e352-035b-4938-bf0c-63410d9bd4cd"), CreatedAt = DateTime.UtcNow, Email = "23573235@sed.com", Password = "test", Role = "user" },
            };
        }

        public sealed class UserDocument : IIdentifiable<Guid>
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Password { get; set; }
            public DateTime CreatedAt { get; set; }
            public IEnumerable<string> Permissions { get; set; }
        }
    }
}
