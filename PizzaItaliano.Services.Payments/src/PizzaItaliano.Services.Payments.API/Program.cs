using Convey;
using Convey.Types;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Payments.Application;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.DTO;
using PizzaItaliano.Services.Payments.Application.Queries;
using PizzaItaliano.Services.Payments.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace PizzaItaliano.Services.Payments.API
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
                        .Get<GetPayments, IEnumerable<PaymentDto>>("payments")
                        .Get<GetPayment, PaymentDto>("payments/{paymentId}")
                        .Post<AddPayment>("payments", afterDispatch: (cmd, ctx) => ctx.Response.Created($"payments/{cmd.PaymentId}"))
                        .Put<UpdatePayment>("payments", afterDispatch: (cmd, ctx) => ctx.Response.Ok($"payments/{cmd.PaymentId}"))
                    ));
    }
}
