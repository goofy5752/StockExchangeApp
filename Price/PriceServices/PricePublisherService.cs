using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;

using RabbitMQ.Client;

using PriceDto.Models;
using PriceServices.Contracts;

using static PriceCommon.GlobalConstants;

namespace PriceServices
{
    public class PricePublisherService : IDisposable, IPricePublisherService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IDistributedCache _cache;

        public PricePublisherService(IDistributedCache cache)
        {
            var factory = new ConnectionFactory() { HostName = HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: PriceExchangeName, type: ExchangeType.Fanout);
            this._cache = cache;
        }

        public async Task PublishPrice(StockPrice price)
        {
            var message = JsonSerializer.Serialize(price);

            var cacheKey = $"{CacheKeyPrefix}{price.Ticker}";

            await _cache.SetStringAsync(cacheKey, message);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: PriceExchangeName,
                                  routingKey: "",
                                  basicProperties: null,
                                  body: body);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}