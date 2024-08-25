using PortfolioData.Common;

namespace PortfolioData.Models
{
    public class Portfolio : BaseModel<int>
    {
        public string UserId { get; set; }

        public List<PortfolioItem> Items { get; set; } = new List<PortfolioItem>();
    }
}