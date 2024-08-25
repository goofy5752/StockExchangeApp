using Microsoft.Extensions.Hosting;

using PriceServices.Contracts;

namespace PriceServices
{
    public class PriceHostedService : IHostedService
    {
        private readonly IPriceGeneratorService _priceGeneratorService;
        private readonly IPricePublisherService _pricePublisherService;
        private Timer _timer;

        public PriceHostedService(IPriceGeneratorService priceGeneratorService, IPricePublisherService pricePublisherService)
        {
            _priceGeneratorService = priceGeneratorService;
            _pricePublisherService = pricePublisherService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(GenerateAndPublishPrices, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void GenerateAndPublishPrices(object state)
        {
            var prices = _priceGeneratorService.GeneratePrices();

            foreach (var price in prices)
            {
                _pricePublisherService.PublishPrice(price);
            }
        }
    }
}