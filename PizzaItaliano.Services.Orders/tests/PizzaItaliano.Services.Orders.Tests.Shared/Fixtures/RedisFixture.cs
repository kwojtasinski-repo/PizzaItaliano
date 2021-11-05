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
            RequestsOptions = OptionsHelper.GetOptions<RequestsOptions>("requests");
            _signalrOptions = OptionsHelper.GetOptions<SignalrOptions>("signalR");
            _redisOptions = OptionsHelper.GetOptions<RedisOptions>("redis");
            //var database = _connectionMultiplexer.GetDatabase(_redisOptions.Database);
            _redisCacheOptions = new RedisCacheOptions() { Configuration = _redisOptions.ConnectionString, InstanceName = _redisOptions.Instance };
            _connectionMultiplexer = ConnectionMultiplexer.Connect(_redisOptions.ConnectionString);
            _redisCache = new RedisCache(_redisCacheOptions);
            DistributedCache = _redisCache;
        }
    }
}
