using PortfolioData.Common;

namespace PortfolioData.Models
{
    public class PortfolioItem : BaseModel<int>
    {
        public string Ticker { get; set; }

        public int Quantity { get; set; }

        public decimal AveragePrice { get; set; }

        public decimal CurrentValue { get; set; }
    }
}