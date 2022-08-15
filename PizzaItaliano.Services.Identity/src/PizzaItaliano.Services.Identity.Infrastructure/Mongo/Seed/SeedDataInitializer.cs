using Convey.Persistence.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity.Infrastructure.Mongo.Seed
{
    internal sealed class SeedDataInitializer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SeedDataInitializer> _logger;

        public SeedDataInitializer(IServiceProvider serviceProvider, ILogger<SeedDataInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var seedDataTypes = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(c => c.GetTypes())
                            .Where(t => DoesTypeSupportInterface(t, typeof(ISeedData<,>)) && !t.IsInterface);

            using var scope = _serviceProvider.CreateScope();
            var seedRepository = scope.ServiceProvider.GetRequiredService<IMongoRepository<SeedDocument, Guid>>();

            foreach(var seedDataType in seedDataTypes)
            {
                var seedData = scope.ServiceProvider.GetService(seedDataType);

                if (seedData is null)
                {
                    _logger.LogInformation($"Cannot find SeedData '{seedDataType.Name}' in container IoC");
                    continue;
                }

                var seedDataNameMethod = seedDataType.GetMethod(nameof(ISeedData<SeedDocument, Guid>.GetSeedDataName));
                var seedMethod = seedDataType.GetMethod(nameof(ISeedData<SeedDocument, Guid>.Seed));
                var seedName = (string)seedDataNameMethod.Invoke(seedData, Array.Empty<object>());
                var seedDataExists = await seedRepository.GetAsync(s => s.SeedDataName == seedName || s.ClassName == seedDataType.FullName);

                if (seedDataExists is not null)
                {
                    var validated = Validate(seedDataExists, seedName);

                    if (!validated)
                    {
                        _logger.LogError($"SeedData with name '{seedName}' was applied and has different name, before change was '{seedDataExists.SeedDataName}'. If you want to apply next changes just create new class and inherit ISeedData");
                        continue;
                    }

                    _logger.LogInformation($"SeedData with name '{seedName}' was applied");
                    continue;
                }

                var entityType = seedDataType.GetInterfaces()[0].GetGenericArguments();
                var mongoRepositoryType = typeof(IMongoRepository<,>);
                mongoRepositoryType = mongoRepositoryType.MakeGenericType(entityType);
                var mongoRepository = scope.ServiceProvider.GetService(mongoRepositoryType);

                if (mongoRepository is null)
                {
                    _logger.LogInformation($"Cannot find MongoRepository '{mongoRepositoryType.Name}' in container IoC");
                    continue;
                }

                var collectionType = mongoRepositoryType.GetProperty(nameof(IMongoRepository<SeedDocument,Guid>.Collection));
                var collection = collectionType.GetValue(mongoRepository);
                var task = (Task) seedMethod.Invoke(seedData, new object[] { collection });
                await task.ConfigureAwait(false);

                await seedRepository.AddAsync(SeedDocument.Create(seedName, seedDataType.FullName, DateTime.UtcNow));
                _logger.LogInformation($"Applied migration for '{seedDataType.Name}' - '{seedName}'");
            }
        }

        private bool Validate(SeedDocument seedDocument, string seedName)
        {
            return seedDocument.SeedDataName == seedName;
        }

        private static bool DoesTypeSupportInterface(Type type, Type inter)
        {
            if (inter.IsAssignableFrom(type))
                return true;
            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == inter))
                return true;
            return false;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
