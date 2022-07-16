using Convey.Persistence.MongoDB;
using PizzaItaliano.Services.Identity.Core.Entities;
using PizzaItaliano.Services.Identity.Core.Repositories;
using PizzaItaliano.Services.Identity.Infrastructure.Mongo.Documents;
using System;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Infrastructure.Mongo.Repositories
{
    internal sealed class UserRepository : IUserRepository
    {
        private readonly IMongoRepository<UserDocument, Guid> _repository;

        public UserRepository(IMongoRepository<UserDocument, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<User> GetAsync(Guid id)
        {
            var user = await _repository.GetAsync(id);

            return user?.AsEntity();
        }

        public async Task<User> GetAsync(string email)
        {
            var user = await _repository.GetAsync(x => x.Email == email.ToLowerInvariant());
            return user?.AsEntity();
        }

        public Task AddAsync(User user) => _repository.AddAsync(user.AsDocument());

        public async Task<User> UpdateAsync(User user)
        {
            var userDocument = user.AsDocument();
            await _repository.UpdateAsync(userDocument);
            return userDocument.AsEntity();
        }
    }
}