using App.Metrics;
using App.Metrics.Gauge;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Infrastructure.Metrics
{
    internal sealed class MetricsJob : BackgroundService
    {
        private readonly GaugeOptions _threads = new GaugeOptions
        {
            Name = "threads"
        };

        private readonly GaugeOptions _workingSet = new GaugeOptions
        {
            Name = "working_set"
        };

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly App.Metrics.MetricsOptions _metricsOptions;
        private readonly ILogger<MetricsJob> _logger;

        public MetricsJob(IServiceScopeFactory serviceScopeFactory, App.Metrics.MetricsOptions metricsOptions,
            ILogger<MetricsJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _metricsOptions = metricsOptions;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_metricsOptions.Enabled)
            {
                _logger.LogInformation("Metics are disabled");
                return;
            }

            while (!stoppingToken.IsCancellationRequested) // dopoki nikt nie anulowal tego taska
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var metricsRoot = scope.ServiceProvider.GetRequiredService<IMetricsRoot>();
                    var process = Process.GetCurrentProcess();
                    metricsRoot.Measure.Gauge.SetValue(_threads, process.Threads.Count);
                    metricsRoot.Measure.Gauge.SetValue(_workingSet, process.WorkingSet64);
                }

                await Task.Delay(5000, stoppingToken); // co 5s wykonuj petle
            }
        }
    }
}
