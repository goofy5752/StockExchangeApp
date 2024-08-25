using PortfolioData.Models;

namespace PortfolioDtos.DtoExtentions
{
    public static class PortfolioMappingExtentions
    {
        public static PortfolioItem ToPortfolioItem(this OrderExecutedDto dto)
        {
            return new PortfolioItem
            {
                Ticker = dto.Ticker,
                Quantity = dto.Quantity,
                AveragePrice = dto.Price,
                CurrentValue = dto.Quantity * dto.Price
            };
        }
    }
}