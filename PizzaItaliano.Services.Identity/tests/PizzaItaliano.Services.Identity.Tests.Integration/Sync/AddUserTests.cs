using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Identity.Tests.Shared.Factories;
using PizzaItaliano.Services.Identity.Tests.Shared.Fixtures;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PizzaItaliano.Services.Identity.Tests.Integration.Sync
{
    [Collection("Collection")]
    public class AddUserTests
    {
        private Task Act(UserDocument document) => _mongoDbFixture.InsertAsync(document);

        [Fact]
        public async Task should_add_user_to_db()
        {
            var userId = Guid.NewGuid();
            var document = new UserDocument { Id = userId, CreatedAt = DateTime.Now, Email = "email@email.com", Password = "password", Role = "user", Permissions = Enumerable.Empty<string>() };

            await Act(document);
            var documentFromDb = await _mongoDbFixture.GetAsync(document.Id);

            documentFromDb.ShouldNotBeNull();
            documentFromDb.ShouldBeOfType<UserDocument>();
        }

        #region Arrange

        private readonly MongoDbFixture<UserDocument, Guid> _mongoDbFixture;

        public AddUserTests(PizzaItalianoApplicationFactory<Program> factory)
        {
            _mongoDbFixture = new MongoDbFixture<UserDocument, Guid>("users");
        }

        #endregion
    }
}
