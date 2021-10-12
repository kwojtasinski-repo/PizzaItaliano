using Convey.CQRS.Queries;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
    internal sealed class GetProductsHandler : IQueryHandler<GetProducts, IEnumerable<ProductDto>>
    {
        private readonly IMongoDatabase _database;

        public GetProductsHandler(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<IEnumerable<ProductDto>> HandleAsync(GetProducts query)
        {
            var collection = _database.GetCollection<ProductDocument>("products");

            if (string.IsNullOrEmpty(query.Name))
            {
                var allProductDocuments = await collection.Find(p => true).ToListAsync();
                var allDtos = allProductDocuments.Select(p => p.AsDto());
                return allDtos;
            }

            var productDocuments = await collection.AsQueryable().Where(p => p.Name.Contains(query.Name)).ToListAsync();
            var dtos = productDocuments.Select(p => p.AsDto());
            return dtos;
        }
    }
}
