using PriceDto.Models;

namespace PriceServices.Contracts
{
    public interface IPricePublisherService
    {
        void PublishPrice(StockPrice price);
    }
}