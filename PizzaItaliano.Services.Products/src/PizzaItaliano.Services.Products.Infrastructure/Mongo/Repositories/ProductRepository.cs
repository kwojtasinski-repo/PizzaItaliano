using Convey.Persistence.MongoDB;
using PizzaItaliano.Services.Products.Core.Entities;
using PizzaItaliano.Services.Products.Core.Repositories;
using PizzaItaliano.Services.Products.Infrastructure.Mongo.Documents;
using System;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Infrastructure.Mongo.Repositories
{
    internal sealed class ProductRepository : IProductRepository
    {
        private readonly IMongoRepository<ProductDocument, Guid> _mongoRepository;

        public ProductRepository(IMongoRepository<ProductDocument, Guid> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public Task AddAsync(Product product)
        {
            var document = product.AsDocument();
            var task = _mongoRepository.AddAsync(document);
            return task;
        }

        public Task DeleteAsync(AggregateId id)
        {
            var task = _mongoRepository.DeleteAsync(id);
            return task;
        }

        public Task<bool> ExistsAsync(AggregateId id)
        {
            var task = _mongoRepository.ExistsAsync(p => p.Id == id);
            return task;
        }

        public async Task<Product> GetAsync(AggregateId id)
        {
            var document = await _mongoRepository.GetAsync(id);
            return document?.AsEntity();
        }

        public Task UpdateAsync(Product product)
        {
            var document = product.AsDocument();
            var task = _mongoRepository.UpdateAsync(document);
            return task;
        }
    }
}
