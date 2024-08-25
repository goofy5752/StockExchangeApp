using PriceDto.Models;

namespace PriceServices.Contracts
{
    public interface IPriceGeneratorService
    {
        IEnumerable<StockPrice> GeneratePrices();
    }
}