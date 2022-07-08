using Newtonsoft.Json;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Identity.Tests.Shared.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Tests.EndToEnd.Helpers
{
    internal static class TestHelper
    {
        public static StringContent GetContent(object value)
            => new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");

        public static T MapTo<T>(string stringResponse)
        {
            var result = JsonConvert.DeserializeObject<T>(stringResponse);
            return result;
        }

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

        public static async Task<RefreshTokenDocument> AddTestRefreshToken(Guid userId, string token, MongoDbFixture<RefreshTokenDocument, Guid> mongoDbFixture)
        {
            var created = DateTime.Now.AddSeconds(-50.5);
            var document = new RefreshTokenDocument { Id = Guid.NewGuid(), Token = token, UserId = userId, CreatedAt = created, RevokedAt = null };

            await mongoDbFixture.InsertAsync(document);

            return document;
        }

        internal sealed class Error
        {
            public string Code { get; set; }
            public string Reason { get; set; }
        }
    }
}
