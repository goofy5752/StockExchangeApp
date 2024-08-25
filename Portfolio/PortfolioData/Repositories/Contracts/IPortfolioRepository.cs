using PortfolioData.Models;

namespace PortfolioData.Repositories.Contracts
{
    public interface IPortfolioRepository
    {
        Task<Portfolio> GetPortfolioByUserIdAsync(string userId);

        Task<IEnumerable<Portfolio>> GetPortfoliosWithTickerAsync(string ticker);

        Task AddPortfolioAsync(Portfolio portfolio);

        void UpdatePortfolio(Portfolio portfolio);

        void UpdatePortfolioItem(PortfolioItem portfolioItem);

        Task SaveChangesAsync();
    }
}