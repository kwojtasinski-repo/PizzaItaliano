using Convey;
using Convey.Persistence.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace PizzaItaliano.Services.Identity.Infrastructure.Mongo.Seed
{
    internal static class Extensions
    {
        public static IConveyBuilder AddSeedData(this IConveyBuilder conveyBuilder)
        {
            conveyBuilder.Services.Scan(s => s.FromCallingAssembly()
                                .AddClasses(c => c.AssignableTo(typeof(ISeedData<,>)))
                                .AsSelf()
                                .WithTransientLifetime());
            conveyBuilder.AddMongoRepository<SeedDocument, Guid>("seeds");
            conveyBuilder.Services.AddHostedService<SeedDataInitializer>();
            return conveyBuilder;
        }
    }
}
