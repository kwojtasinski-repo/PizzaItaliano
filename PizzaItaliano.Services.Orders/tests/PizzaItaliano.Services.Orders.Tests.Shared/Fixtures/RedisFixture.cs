using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using PizzaItaliano.Services.Orders.Tests.Shared.Helpers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Tests.Shared.Fixtures
{
    public class RedisFixture
    {
        public RequestsOptions RequestsOptions { get; }
        private readonly SignalrOptions _signalrOptions;
        private readonly RedisOptions _redisOptions;
        private readonly RedisCacheOptions _redisCacheOptions;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly RedisCache _redisCache;
        public IDistributedCache DistributedCache { get; }

        public RedisFixture()
        {
            RequestsOptions = OptionsHelper.GetOptions<RequestsOptions>("requests"); // opcje z appsettings
            _signalrOptions = OptionsHelper.GetOptions<SignalrOptions>("signalR"); // opcje z appsettings
            _redisOptions = OptionsHelper.GetOptions<RedisOptions>("redis"); // opcje z appsettings
            _redisCacheOptions = new RedisCacheOptions() { Configuration = _redisOptions.ConnectionString, InstanceName = _redisOptions.Instance };
            _connectionMultiplexer = ConnectionMultiplexer.Connect(_redisOptions.ConnectionString); // polaczenie z redis
            _redisCache = new RedisCache(_redisCacheOptions);
            DistributedCache = _redisCache;
        }
    }
}
