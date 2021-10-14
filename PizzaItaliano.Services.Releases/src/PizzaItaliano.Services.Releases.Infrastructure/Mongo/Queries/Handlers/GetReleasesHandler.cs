using Convey.CQRS.Queries;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PizzaItaliano.Services.Releases.Application.DTO;
using PizzaItaliano.Services.Releases.Application.Queries;
using PizzaItaliano.Services.Releases.Infrastructure.Mongo.Documents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Infrastructure.Mongo.Queries.Handlers
{
    public class GetReleasesHandler : IQueryHandler<GetReleases, IEnumerable<ReleaseDto>>
    {
        private readonly IMongoDatabase _mongoDatabase;

        public GetReleasesHandler(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task<IEnumerable<ReleaseDto>> HandleAsync(GetReleases query)
        {
            var collection = _mongoDatabase.GetCollection<ReleaseDocument>("releases");

            if (!query.OrderId.HasValue)
            {
                var allReleaseDocuments = await collection.Find(_ => true).ToListAsync();
                var releaseDocumentsDto = allReleaseDocuments.Select(r => r.AsDto());

                return releaseDocumentsDto;
            }

            var documents = collection.AsQueryable();
            var releaseDocuments = await documents.Where(r => r.OrderId == query.OrderId.Value).ToListAsync();
            var releaseDtos = releaseDocuments.Select(r => r.AsDto());

            return releaseDtos;
        }
    }
}
