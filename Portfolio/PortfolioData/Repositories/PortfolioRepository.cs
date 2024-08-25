using Microsoft.EntityFrameworkCore;

using PortfolioData.Models;
using PortfolioData.Repositories.Contracts;

namespace PortfolioData.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly PortfolioDbContext _context;

        public PortfolioRepository(PortfolioDbContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> GetPortfolioByUserIdAsync(string userId)
        {
            return await _context.Portfolios
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<Portfolio>> GetPortfoliosWithTickerAsync(string ticker)
        {
            return await _context.Portfolios
                .Include(p => p.Items)
                .Where(p => p.Items.Any(i => i.Ticker.ToLower() == ticker.ToLower()))
                .ToListAsync();
        }

        public async Task AddPortfolioAsync(Portfolio portfolio)
        {
            await _context.Portfolios.AddAsync(portfolio);
        }

        public void UpdatePortfolio(Portfolio portfolio)
        {
            _context.Portfolios.Update(portfolio);
        }

        public void UpdatePortfolioItem(PortfolioItem portfolioItem)
        {
            _context.PortfolioItems.Update(portfolioItem);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}