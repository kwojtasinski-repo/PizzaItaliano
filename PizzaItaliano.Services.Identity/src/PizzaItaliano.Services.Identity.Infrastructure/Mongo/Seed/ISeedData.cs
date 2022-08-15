using Convey.Types;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Infrastructure.Mongo.Seed
{
    // marker
    internal interface ISeedData<TEntity, TIdentifiable>
        where TEntity : class, IIdentifiable<TIdentifiable>
    {
        public string GetSeedDataName();
        public Task Seed(IMongoCollection<TEntity> collection);
    }
}
