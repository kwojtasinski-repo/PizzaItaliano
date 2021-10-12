using Convey.CQRS.Queries;
using MongoDB.Driver;
using PizzaItaliano.Services.Products.Application.DTO;
using PizzaItaliano.Services.Products.Application.Queries;
using PizzaItaliano.Services.Products.Infrastructure.Mongo.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Products.Infrastructure.Mongo.Queries.Handlers
{
    internal sealed class GetProductHandler : IQueryHandler<GetProduct, ProductDto>
    {
        private readonly IMongoDatabase _database;

        public GetProductHandler(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<ProductDto> HandleAsync(GetProduct query)
        {
            var product = await _database.GetCollection<ProductDocument>("products")
                .Find(p => p.Id == query.ProductId)
                .SingleOrDefaultAsync();

            var productDto = product?.AsDto();

            return productDto;
        }
    }
}
