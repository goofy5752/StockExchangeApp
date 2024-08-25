using PriceDto.Models;
using PriceServices.Contracts;

using static PriceCommon.GlobalConstants;

namespace PriceServices
{
    public class PriceGeneratorService : IPriceGeneratorService
    {
        public IEnumerable<StockPrice> GeneratePrices()
        {
            var random = new Random();
            var prices = new List<StockPrice>();

            foreach (var ticker in Tickers)
            {
                var price = random.Next(100, 1500) + random.NextDouble();
                prices.Add(new StockPrice
                {
                    Ticker = ticker,
                    Price = Convert.ToDecimal(price),
                });
            }

            return prices;
        }
    }
}