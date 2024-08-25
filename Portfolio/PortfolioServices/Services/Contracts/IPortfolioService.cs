using PortfolioDtos;

namespace PortfolioServices.Services.Contracts
{
    public interface IPortfolioService
    {
        Task ProcessOrderExecutedAsync(OrderExecutedDto orderExecutedDto);

        Task UpdatePortfolioValueAsync(PriceUpdatedDto priceUpdatedDto);
    }
}