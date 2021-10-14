using Convey.CQRS.Queries;
using MongoDB.Driver;
using PizzaItaliano.Services.Releases.Application.DTO;
using PizzaItaliano.Services.Releases.Application.Queries;
using PizzaItaliano.Services.Releases.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Releases.Infrastructure.Mongo.Queries.Handlers
{
    internal sealed class GetReleaseHandler : IQueryHandler<GetRelease, ReleaseDto>
    {
        private readonly IMongoDatabase _mongoRepository;

        public GetReleaseHandler(IMongoDatabase mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public async Task<ReleaseDto> HandleAsync(GetRelease query)
        {
            var releaseDocument = await _mongoRepository.GetCollection<ReleaseDocument>("releases")
                .Find(r => r.Id == query.ReleaseId)
                .SingleOrDefaultAsync();

            return releaseDocument?.AsDto();
        }
    }
}
