using System.Text;
using System.Text.Json;

using RabbitMQ.Client;

using OrderDtos;
using OrderServices.Services.Contracts;

using static OrderCommon.GlobalConstants;

namespace OrderServices.Services
{
    public class OrderPublisherService : IDisposable, IOrderPublisherService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public OrderPublisherService()
        {
            var factory = new ConnectionFactory() { HostName = HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: OrderExchangeName, ExchangeType.Fanout, durable: true, autoDelete: false);
        }

        public void PublishOrderExecuted(OrderExecutedDto orderExecutedDto)
        {
            var message = JsonSerializer.Serialize(orderExecutedDto);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: OrderExchangeName,
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