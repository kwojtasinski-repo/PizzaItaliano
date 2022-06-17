using Convey.Persistence.MongoDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace PizzaItaliano.Services.Payments.Tests.Shared.Factories
{
    public class PizzaItalianoApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
            => base.CreateWebHostBuilder().UseEnvironment("tests");

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
