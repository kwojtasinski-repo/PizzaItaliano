﻿using Convey.MessageBrokers.RabbitMQ;
using Newtonsoft.Json;
using PizzaItaliano.Services.Payments.Tests.Shared.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PizzaItaliano.Services.Payments.Tests.Shared.Fixtures
{
    public class RabbitMqFixture
    {
        private readonly IModel _channel;
        private bool _disposed = false;

        public RabbitMqFixture()
        {
            var options = OptionsHelper.GetOptions<RabbitMqOptions>("rabbitMq");
            var connectionFactory = new ConnectionFactory
            {
                HostName = options.HostNames?.FirstOrDefault(
                    ),
                VirtualHost = options.VirtualHost,
                Port = options.Port,
                UserName = options.Username,
                Password = options.Password,
                UseBackgroundThreadsForIO = true,
                DispatchConsumersAsync = true,
                Ssl = new SslOption()
            };

            var connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
        }

        public Task PublishAsync<TMessage>(TMessage message, string exchange = null) where TMessage : class
        {
            return PublishAsync(message, exchange, null);
        }
        
        public Task PublishAsync<TMessage>(TMessage message, string exchange = null, IDictionary<string, object> headers = null) where TMessage : class
        {
            var routingKey = SnakeCase(message.GetType().Name);
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            var properties = _channel.CreateBasicProperties();
            properties.Headers = headers ?? new Dictionary<string, object>();
            properties.MessageId = Guid.NewGuid().ToString();
            properties.CorrelationId = Guid.NewGuid().ToString();
            _channel.BasicPublish(exchange, routingKey, properties, body);
            return Task.CompletedTask;
        }

        public TaskCompletionSource<TEntity> SubscribeAndGet<TMessage, TEntity>(string exchange,
            Func<Guid, TaskCompletionSource<TEntity>, Task> onMessageReceived, Guid id)
        {
            var taskCompletionSource = new TaskCompletionSource<TEntity>();

            _channel.ExchangeDeclare(exchange: exchange,
                durable: true,
                autoDelete: false,
                arguments: null,
                type: "topic");

            var queue = $"test_{SnakeCase(typeof(TMessage).Name)}_{Guid.NewGuid().ToString("N")}";

            _channel.QueueDeclare(queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(queue, exchange, SnakeCase(typeof(TMessage).Name));
            _channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var json = Encoding.UTF8.GetString(body.Span);
                var message = JsonConvert.DeserializeObject<TMessage>(json);

                await onMessageReceived(id, taskCompletionSource);
            };

            _channel.BasicConsume(queue: queue,
                autoAck: true,
                consumer: consumer);

            return taskCompletionSource;
        }

        public TaskCompletionSource<TEntity> SubscribeAndGet<TMessage, TEntity>(string exchange,
            Func<Expression<Func<TEntity, bool>>, TaskCompletionSource<TEntity>, Task> onMessageReceived, Expression<Func<TEntity, bool>> filter)
        {
            var taskCompletionSource = new TaskCompletionSource<TEntity>();

            _channel.ExchangeDeclare(exchange: exchange,
                durable: true,
                autoDelete: false,
                arguments: null,
                type: "topic");

            var queue = $"test_{SnakeCase(typeof(TMessage).Name)}_{Guid.NewGuid().ToString("N")}";

            _channel.QueueDeclare(queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(queue, exchange, SnakeCase(typeof(TMessage).Name));
            _channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var json = Encoding.UTF8.GetString(body.Span);
                var message = JsonConvert.DeserializeObject<TMessage>(json);

                await onMessageReceived(filter, taskCompletionSource);
            };

            _channel.BasicConsume(queue: queue,
                autoAck: true,
                consumer: consumer);

            return taskCompletionSource;
        }

        public TaskCompletionSource<TMessage> SubscribeAndGet<TMessage>(string exchange)
        {
            var taskCompletionSource = new TaskCompletionSource<TMessage>();

            _channel.ExchangeDeclare(exchange: exchange,
                durable: true,
                autoDelete: false,
                arguments: null,
                type: "topic");

            var queue = $"test_{SnakeCase(typeof(TMessage).Name)}_{Guid.NewGuid().ToString("N")}";

            _channel.QueueDeclare(queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(queue, exchange, SnakeCase(typeof(TMessage).Name));
            _channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var json = Encoding.UTF8.GetString(body.Span);
                var message = JsonConvert.DeserializeObject<TMessage>(json);

                if (message is null)
                {
                    await Task.Run(() => taskCompletionSource.TrySetCanceled());
                }

                await Task.Run(() => taskCompletionSource.TrySetResult(message));
            };

            _channel.BasicConsume(queue: queue,
                autoAck: true,
                consumer: consumer);

            return taskCompletionSource;
        }

        private static string SnakeCase(string value)
            => string.Concat(value.Select((x, i) =>
                    i > 0 && value[i - 1] != '.' && value[i - 1] != '/' && char.IsUpper(x) ? "_" + x : x.ToString()))
                .ToLowerInvariant();

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _channel.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
