using Convey.Persistence.MongoDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using PizzaItaliano.Services.Identity.Tests.Shared.Initializer;

namespace PizzaItaliano.Services.Identity.Tests.Shared.Factories
{
    public class PizzaItalianoApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = base.CreateWebHostBuilder().UseEnvironment("tests");
            builder.ConfigureServices(services =>
            {
                services.AddHostedService<UserInitializer>();
            });
            return builder;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var options = Services.GetRequiredService<MongoDbOptions>();
                var client = new MongoClient(options.ConnectionString);
                client.DropDatabase(options.Database);
            }

            base.Dispose(disposing);
        }
    }
}