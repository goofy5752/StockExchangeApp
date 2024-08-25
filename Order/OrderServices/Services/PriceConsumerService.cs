using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;

using Microsoft.Extensions.Hosting;

using RabbitMQ.Client.Events;
using RabbitMQ.Client;

using OrderDtos;

using static OrderCommon.GlobalConstants;

namespace OrderServices.Services
{
    public class PriceConsumerService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ConcurrentDictionary<string, decimal> _latestPrices;

        public PriceConsumerService(ConcurrentDictionary<string, decimal> latestPrices)
        {
            _latestPrices = latestPrices;
            var factory = new ConnectionFactory() { HostName = HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: PriceExchangeName, type: ExchangeType.Fanout);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            _channel.QueueDeclare(queue: PriceUpdateQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: PriceUpdateQueueName, exchange: PriceExchangeName, routingKey: "");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var stockPrice = JsonSerializer.Deserialize<StockPriceDto>(content);

                if (stockPrice != null)
                {
                    _latestPrices.AddOrUpdate(stockPrice.Ticker, stockPrice.Price, (key, oldValue) => stockPrice.Price);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: PriceUpdateQueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}