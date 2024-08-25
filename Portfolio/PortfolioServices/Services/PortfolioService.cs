using PortfolioData.Models;
using PortfolioData.Repositories.Contracts;
using PortfolioDtos;
using PortfolioDtos.DtoExtentions;
using PortfolioServices.Services.Contracts;

using static PortfolioCommon.GlobalConstants;

namespace PortfolioServices.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public PortfolioService(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        public async Task ProcessOrderExecutedAsync(OrderExecutedDto orderExecutedDto)
        {
            await _semaphore.WaitAsync();

            try
            {
                var portfolio = await _portfolioRepository.GetPortfolioByUserIdAsync(orderExecutedDto.UserId);

                if (portfolio == null)
                {
                    portfolio = new Portfolio { UserId = orderExecutedDto.UserId };
                    await _portfolioRepository.AddPortfolioAsync(portfolio);

                    await _portfolioRepository.SaveChangesAsync();
                }

                var portfolioItem = portfolio.Items.FirstOrDefault(i => i.Ticker == orderExecutedDto.Ticker);
                portfolioItem = AddOrUpdatePortfolio(orderExecutedDto, portfolio, portfolioItem);

                _portfolioRepository.UpdatePortfolio(portfolio);

                await _portfolioRepository.SaveChangesAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task UpdatePortfolioValueAsync(PriceUpdatedDto priceUpdatedDto)
        {
            await _semaphore.WaitAsync();

            try
            {
                var portfolios = await _portfolioRepository.GetPortfoliosWithTickerAsync(priceUpdatedDto.Ticker);
                foreach (var portfolio in portfolios)
                {
                    var portfolioItem = portfolio.Items.FirstOrDefault(i => i.Ticker == priceUpdatedDto.Ticker);
                    if (portfolioItem != null)
                    {
                        UpdatePortfolioItem(portfolioItem, priceUpdatedDto);
                        _portfolioRepository.UpdatePortfolioItem(portfolioItem);
                    }
                }

                await _portfolioRepository.SaveChangesAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private static PortfolioItem AddOrUpdatePortfolio(OrderExecutedDto orderExecutedDto, Portfolio portfolio, PortfolioItem? portfolioItem)
        {
            if (portfolioItem == null)
            {
                if (orderExecutedDto.Side.Equals(OrderSellSide, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                if (orderExecutedDto.Side.Equals(OrderBuySide, StringComparison.OrdinalIgnoreCase))
                {
                    portfolioItem = orderExecutedDto.ToPortfolioItem();
                    portfolio.Items.Add(portfolioItem);
                }
            }
            else
            {
                if (orderExecutedDto.Side.Equals(OrderBuySide, StringComparison.OrdinalIgnoreCase))
                {
                    var totalQuantity = portfolioItem.Quantity + orderExecutedDto.Quantity;

                    portfolioItem.AveragePrice = ((portfolioItem.AveragePrice * portfolioItem.Quantity) + (orderExecutedDto.Price * orderExecutedDto.Quantity)) / totalQuantity;

                    portfolioItem.Quantity = totalQuantity;
                }
                else if (orderExecutedDto.Side.Equals(OrderSellSide, StringComparison.OrdinalIgnoreCase))
                {
                    portfolioItem.Quantity -= orderExecutedDto.Quantity;

                    if (portfolioItem.Quantity <= 0)
                    {
                        portfolioItem.Quantity = 0;
                        portfolio.Items.Remove(portfolioItem);
                        return portfolioItem;
                    }
                }
            }

            return portfolioItem;
        }

        private static void UpdatePortfolioItem(PortfolioItem portfolioItem, PriceUpdatedDto dto)
        {
            portfolioItem.CurrentValue = portfolioItem.Quantity * dto.Price;
        }
    }
}