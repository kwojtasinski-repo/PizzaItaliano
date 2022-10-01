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
using Convey.Logging;
using Convey.Secrets.Vault;
using System.Net;
using System;
using PizzaItaliano.Services.Payments.Application.Exceptions;

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
                        .Get("", ctx => ctx.Response.WriteAsJsonAsync(ctx.RequestServices.GetService<AppOptions>().Name))
                        .Get<GetPayments, IEnumerable<PaymentDto>>("payments")
                        .Get<GetPayments, IEnumerable<PaymentDto>>("payments/{paymentStatus:int}")
                        .Get<GetPaymentsWithDateAndStatus, IEnumerable<PaymentDto>>("payments/from/{dateFrom}/to/{dateTo}")
                        .Get<GetPaymentsWithDateAndStatus, IEnumerable<PaymentDto>>("payments/from/{dateFrom}/to/{dateTo}/{paymentStatus:int}")
                        .Get<GetPayment, PaymentDto>("payments/{paymentId}", afterDispatch: (cmd, result, ctx) =>
                        {
                            if (result is null)
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                var task = ctx.Response.WriteAsJsonAsync(new
                                {
                                    code = (int)HttpStatusCode.NotFound,
                                    reason = $"Payment with id {cmd.PaymentId} was not found"
                                });

                                return task;
                            }

                            return ctx.Response.Ok(result);
                        })
                        .Get<GetPaymentByOrderId, PaymentDto>("payments/by-order/{orderId}", afterDispatch: (cmd, result, ctx) =>
                        {
                            if (result is null)
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                var task = ctx.Response.WriteAsJsonAsync(new
                                {
                                    code = (int)HttpStatusCode.NotFound,
                                    reason = $"Payment for order with id {cmd.OrderId} was not found"
                                });

                                return task;
                            }

                            return ctx.Response.Ok(result);
                        })
                        .Post<AddPayment>("payments", afterDispatch: (cmd, ctx) => ctx.Response.Created($"payments/{cmd.PaymentId}"))
                        .Put<PayFromPayment>("payments/{paymentId}/pay", beforeDispatch: async (cmd, ctx) =>
                        {
                            var isValid = Guid.TryParse(ctx.Request.RouteValues["paymentId"] as string, out var paymentId);

                            if (!isValid)
                            {
                                throw new InvalidPaymentIdException();
                            }

                            cmd.PaymentId = paymentId;
                        }, afterDispatch: (cmd, ctx) => ctx.Response.Ok($"payments/{cmd.PaymentId}"))
                    ))
                .UseLogging()
                .UseVault();
    }
}
