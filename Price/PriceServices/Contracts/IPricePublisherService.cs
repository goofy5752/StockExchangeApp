using PriceDto.Models;

namespace PriceServices.Contracts
{
    public interface IPricePublisherService
    {
        Task PublishPrice(StockPrice price);
    }
}