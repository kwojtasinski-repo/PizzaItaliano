using Convey.Types;
using System;

namespace PizzaItaliano.Services.Identity.Infrastructure.Mongo.Seed
{
    internal sealed class SeedDocument : IIdentifiable<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string SeedDataName { get; set; }
        public string ClassName { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        private SeedDocument(Guid id, string seedDataName, string className, DateTime created)
        {
            Id = id;
            SeedDataName = seedDataName;
            ClassName = className;
            Created = created;
        }

        public static SeedDocument Create(string seedDataName, string className, DateTime created)
        {
            return new SeedDocument(Guid.NewGuid(), seedDataName, className, created);
        }
    }
}
