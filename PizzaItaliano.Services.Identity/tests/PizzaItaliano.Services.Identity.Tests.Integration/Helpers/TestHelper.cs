using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Identity.Tests.Shared.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Tests.Integration.Helpers
{
    internal static class TestHelper
    {
        public static Task AddTestUser(Guid userId, MongoDbFixture<UserDocument, Guid> mongoDbFixture)
        {
            var created = DateTime.Now;
            var document = new UserDocument { Id = userId, CreatedAt = created, Email = "user@user.com", Password = "password", Role = "admin", Permissions = Enumerable.Empty<string>() };

            return mongoDbFixture.InsertAsync(document);
        }

        public static Task AddTestUser(Guid userId, string email, string password, MongoDbFixture<UserDocument, Guid> mongoDbFixture)
        {
            var created = DateTime.Now;
            var document = new UserDocument { Id = userId, CreatedAt = created, Email = email, Password = password, Role = "admin", Permissions = Enumerable.Empty<string>() };

            return mongoDbFixture.InsertAsync(document);
        }

        public static Task AddTestUser(Guid userId, string email, string password, string role, MongoDbFixture<UserDocument, Guid> mongoDbFixture, IEnumerable<string> permissions = null)
        {
            var created = DateTime.Now;
            var document = new UserDocument { Id = userId, CreatedAt = created, Email = email, Password = password, Role = role, Permissions = permissions ?? Enumerable.Empty<string>() };

            return mongoDbFixture.InsertAsync(document);
        }
    }
}