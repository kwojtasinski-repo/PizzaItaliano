using Convey.Persistence.MongoDB;
using PizzaItaliano.Services.Releases.Core.Entities;
using PizzaItaliano.Services.Releases.Core.Repositories;
using PizzaItaliano.Services.Releases.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Infrastructure.Mongo.Repositories
{
    internal sealed class ReleaseRepository : IReleaseRepository
    {
        private readonly IMongoRepository<ReleaseDocument, Guid> _mongoRepository;

        public ReleaseRepository(IMongoRepository<ReleaseDocument, Guid> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public Task AddAsync(Release release)
        {
            var releaseDocument = release.AsDocument();
            var task = _mongoRepository.AddAsync(releaseDocument);
            return task;
        }

        public Task DeleteAsync(AggregateId id)
        {
            var task = _mongoRepository.DeleteAsync(id);
            return task;
        }

        public Task<bool> ExistsAsync(AggregateId id)
        {
            var exists = _mongoRepository.ExistsAsync(r => r.Id == id);
            return exists;
        }

        public async Task<IEnumerable<Release>> GetAllByOrderIdAsync(Guid orderId)
        {
            var releasesDocuments = await _mongoRepository.FindAsync(r => r.OrderId == orderId);
            return releasesDocuments.Select(r => r.AsEntity());
        }

        public async Task<Release> GetAsync(AggregateId id)
        {
            var releaseDocument = await _mongoRepository.GetAsync(id);
            var release = releaseDocument.AsEntity();
            return release;
        }

        public Task UpdateAsync(Release release)
        {
            var releaseDocument = release.AsDocument();
            var task = _mongoRepository.UpdateAsync(releaseDocument);
            return task;
        }
    }
}
