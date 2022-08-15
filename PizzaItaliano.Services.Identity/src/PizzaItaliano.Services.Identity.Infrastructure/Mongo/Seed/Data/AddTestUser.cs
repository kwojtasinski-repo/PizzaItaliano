using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using PizzaItaliano.Services.Identity.Application.Services;
using PizzaItaliano.Services.Identity.Infrastructure.Auth;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using System;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Infrastructure.Mongo.Seed.Data
{
    internal class AddTestUser : ISeedData<UserDocument, Guid>
    {
        public string GetSeedDataName()
        {
            return "AddTestUser_2022_08_15_13_30";
        }

        public async Task Seed(IMongoCollection<UserDocument> collection)
        {
            var passwordService = new PasswordService(new PasswordHasher<IPasswordService>());

            await collection.InsertOneAsync(new UserDocument
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Email = "admin@admin.com",
                Password = passwordService.Hash("PasW0Rd!26"),
                Role = "admin",
                Permissions = Array.Empty<string>()
            });
        }
    }
}
