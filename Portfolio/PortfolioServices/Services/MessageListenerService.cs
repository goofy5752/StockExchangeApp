using System.Text;
using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using PortfolioDtos;
using PortfolioServices.Services.Contracts;

using static PortfolioCommon.GlobalConstants;

namespace PortfolioServices.Services
{
    public class MessageListenerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public MessageListenerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory() { HostName = HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: OrderExchangeName, type: ExchangeType.Fanout, durable: true, autoDelete: false);
            _channel.ExchangeDeclare(exchange: PriceExchangeName, type: ExchangeType.Fanout, durable: false, autoDelete: false);

            _channel.QueueDeclare(queue: OrderUpdateQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: OrderUpdateQueueName, exchange: OrderExchangeName, routingKey: "");

            _channel.QueueDeclare(queue: PriceUpdateQueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue: PriceUpdateQueueName, exchange: PriceExchangeName, routingKey: "");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var orderConsumer = new EventingBasicConsumer(_channel);
            orderConsumer.Received += async (model, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();

                var portfolioService = scope.ServiceProvider.GetRequiredService<IPortfolioService>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var orderExecutedDto = JsonSerializer.Deserialize<OrderExecutedDto>(message);
                    if (orderExecutedDto != null)
                    {
                        await portfolioService.ProcessOrderExecutedAsync(orderExecutedDto);
                    }
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception)
                {
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };
            _channel.BasicConsume(queue: OrderUpdateQueueName, autoAck: false, consumer: orderConsumer);

            var priceConsumer = new EventingBasicConsumer(_channel);
            priceConsumer.Received += async (model, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();

                var portfolioService = scope.ServiceProvider.GetRequiredService<IPortfolioService>();

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var priceUpdatedDto = JsonSerializer.Deserialize<PriceUpdatedDto>(message);

                    if (priceUpdatedDto != null)
                    {
                        await portfolioService.UpdatePortfolioValueAsync(priceUpdatedDto);
                    }
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception)
                {
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                }
            };
            _channel.BasicConsume(queue: PriceUpdateQueueName, autoAck: false, consumer: priceConsumer);

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