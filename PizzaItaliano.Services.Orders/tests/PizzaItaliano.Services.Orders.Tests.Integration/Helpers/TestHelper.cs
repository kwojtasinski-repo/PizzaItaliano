﻿using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PizzaItaliano.Services.Orders.Infrastructure.Mongo.Documents;
using PizzaItaliano.Services.Orders.Tests.Shared.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Orders.Tests.Integration.Helpers
{
    internal static class TestHelper
    {
        public static Task AddTestOrder(Guid orderId, MongoDbFixture<OrderDocument, Guid> mongoDbFixture)
        {
            var cost = new decimal(100);
            var orderDate = DateTime.Now;
            var orderNumber = "ORD/2021/11/03/1";
            var status = Core.Entities.OrderStatus.Released;
            var releasedDate = DateTime.Now;
            var document = new OrderDocument { Id = orderId, Cost = cost, OrderDate = orderDate, OrderNumber = orderNumber, OrderStatus = status, ReleaseDate = releasedDate, Version = 0 };

            return mongoDbFixture.InsertAsync(document);
        }

        public static Task AddTestOrder(Guid orderId, Core.Entities.OrderStatus status, MongoDbFixture<OrderDocument, Guid> mongoDbFixture)
        {
            var cost = new decimal(100);
            var orderDate = DateTime.Now;
            var orderNumber = "ORD/2021/11/03/1";
            var releasedDate = DateTime.Now;
            var document = new OrderDocument { Id = orderId, Cost = cost, OrderDate = orderDate, OrderNumber = orderNumber, OrderStatus = status, ReleaseDate = releasedDate, Version = 0 };

            return mongoDbFixture.InsertAsync(document);
        }

        public static Task AddTestOrderWithOrderProduct(Guid orderId, Guid orderProductId, MongoDbFixture<OrderDocument, Guid> mongoDbFixture)
        {
            var cost = new decimal(100);
            var orderDate = DateTime.Now;
            var orderNumber = "ORD/2021/11/03/1";
            var status = Core.Entities.OrderStatus.Released;
            var orderProductStatus = Core.Entities.OrderProductStatus.Released;
            var productId = Guid.NewGuid();
            var orderProductDocument = new OrderProductDocument { Id = orderProductId, Cost = cost, OrderId = orderId, Quantity = 1, OrderProductStatus = orderProductStatus, ProductId = productId };
            var products = new List<OrderProductDocument> { orderProductDocument };
            var releasedDate = DateTime.Now;
            var document = new OrderDocument { Id = orderId, Cost = cost, OrderDate = orderDate, OrderNumber = orderNumber, OrderStatus = status, OrderProductDocuments = products, ReleaseDate = releasedDate, Version = 0 };

            return mongoDbFixture.InsertAsync(document);
        }

        public static Task AddTestOrderWithOrderProduct(Guid orderId, Guid orderProductId, Core.Entities.OrderStatus orderStatus, Core.Entities.OrderProductStatus orderProductStatus, MongoDbFixture<OrderDocument, Guid> mongoDbFixture)
        {
            var cost = new decimal(100);
            var orderDate = DateTime.Now;
            var orderNumber = "ORD/2021/11/03/1";
            var productId = Guid.NewGuid();
            var orderProductDocument = new OrderProductDocument { Id = orderProductId, Cost = cost, OrderId = orderId, Quantity = 1, OrderProductStatus = orderProductStatus, ProductId = productId };
            var products = new List<OrderProductDocument> { orderProductDocument };
            var releasedDate = DateTime.Now;
            var document = new OrderDocument { Id = orderId, Cost = cost, OrderDate = orderDate, OrderNumber = orderNumber, OrderStatus = orderStatus, OrderProductDocuments = products, ReleaseDate = releasedDate, Version = 0 };

            return mongoDbFixture.InsertAsync(document);
        }

        public static async Task AddObjectToCache<T>(this RedisFixture redisFixture, T obj, string name)
        {
            var objectSerialized = JsonConvert.SerializeObject(obj);
            var expirySeconds = redisFixture.RequestsOptions.ExpirySeconds;
            await redisFixture.DistributedCache.SetStringAsync(GetKey(name),
                objectSerialized,
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(expirySeconds)
                });
        }

        private static string GetKey(string id) => $"requests:{id}";
    }
}
