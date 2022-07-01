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
using PizzaItaliano.Services.Orders.Application;
using PizzaItaliano.Services.Orders.Application.Commands;
using PizzaItaliano.Services.Orders.Application.DTO;
using PizzaItaliano.Services.Orders.Application.Queries;
using PizzaItaliano.Services.Orders.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using Convey.Logging;
using Convey.Secrets.Vault;
using System.Net;

namespace PizzaItaliano.Services.Orders.API
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
                            .Get<GetOrders, IEnumerable<OrderDto>>("orders")
                            .Get<GetOrder, OrderDto>("orders/{orderId}", afterDispatch: (cmd, result, ctx) =>
                            {
                                if (result is null)
                                {
                                    ctx.Response.StatusCode = (int) HttpStatusCode.NotFound;
                                    var task = ctx.Response.WriteAsJsonAsync(new
                                    {
                                        code = (int) HttpStatusCode.NotFound,
                                        reason = $"Order with id {cmd.OrderId} was not found"
                                    });

                                    return task;
                                }

                                return ctx.Response.Ok(result);
                            })
                            .Post<AddOrder>("orders", afterDispatch: (cmd, ctx) => ctx.Response.Created($"orders/{cmd.OrderId}"))
                            .Post<AddOrderProduct>("orders/order-product", afterDispatch: (cmd, ctx) => ctx.Response.Ok($"orders/order-product/{cmd.OrderProductId}"))
                            .Put<SetOrderStatusReady>("orders", afterDispatch: (cmd, ctx) => ctx.Response.Ok($"Set status ready orders/{cmd.OrderId}"))
                            .Delete<DeleteOrderProduct>("orders/{orderId}/order-product/{orderProductId}/quantity/{quantity:int}", afterDispatch: (cmd, ctx) => ctx.Response.Ok($"Deleted orders/order-product/{cmd.OrderProductId} with quantity {cmd.Quantity}"))
                        ))
                    .UseLogging()
                    .UseVault();
    }
}
