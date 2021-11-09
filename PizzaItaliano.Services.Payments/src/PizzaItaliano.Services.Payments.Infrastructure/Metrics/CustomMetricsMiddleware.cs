using App.Metrics;
using App.Metrics.Counter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PizzaItaliano.Services.Payments.Application.Commands;
using PizzaItaliano.Services.Payments.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Infrastructure.Metrics
{
    internal sealed class CustomMetricsMiddleware : IMiddleware
    {
        private readonly IDictionary<string, CounterOptions> _metrics = new Dictionary<string, CounterOptions>
        {
            [GetKey("GET", "/payments")] = Query(typeof(GetPayments).Name),
            [GetKey("POST", "/payments")] = Command(typeof(AddPayment).Name),
            [GetKey("PUT", "/payments")] = Command(typeof(UpdatePayment).Name)
        };

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly bool _enabled;


        public CustomMetricsMiddleware(IServiceScopeFactory serviceScopeFactory, MetricsOptions metricsOptions)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _enabled = metricsOptions.Enabled;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!_enabled)
            {
                return next(context);
            }

            var request = context.Request;
            if (!_metrics.TryGetValue(GetKey(request.Method, request.Path.ToString()), out var metrics))
            {
                return next(context);
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var metricsRoot = scope.ServiceProvider.GetRequiredService<IMetricsRoot>();
            metricsRoot.Measure.Counter.Increment(metrics);

            return next(context);
        }

        private static string GetKey(string method, string path)
        {
            return $"{method}:{path}";
        }

        private static CounterOptions Command(string command)
        {
            var options = new CounterOptions
            {
                Name = "commands",
                Tags = new App.Metrics.MetricTags("command", command)
            };

            return options;
        }

        private static CounterOptions Query(string query)
        {
            var options = new CounterOptions
            {
                Name = "queries",
                Tags = new App.Metrics.MetricTags("query", query)
            };

            return options;
        }
    }
}
