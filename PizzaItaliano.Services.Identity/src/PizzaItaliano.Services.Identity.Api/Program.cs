using Convey;
using Convey.Logging;
using Convey.Secrets.Vault;
using Convey.Types;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PizzaItaliano.Services.Identity.Application;
using PizzaItaliano.Services.Identity.Infrastructure;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Identity
{
    public class Program
    {
        public static async Task Main(string[] args)
            => await CreateWebHostBuilder(args).Build().RunAsync();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                    .ConfigureServices(services => services
                        .AddConvey()
                        .AddWebApi()
                        .AddApplication()
                        .AddInfrastructure()
                        .Build())
                    .Configure(app => app
                        .UseInfrastructure()
                        .UseDispatcherEndpoints(endpoints => endpoints
                            .Get("", ctx => ctx.Response.WriteAsync(ctx.RequestServices.GetService<AppOptions>().Name))
                        ))
                    .UseLogging()
                    .UseVault();
    }
}
