using OrderData.Common;

namespace OrderData.Models
{
    public class Order : BaseModel<int>
    {
        public string Ticker { get; set; }

        public int Quantity { get; set; }

        public string Side { get; set; }

        public decimal Price { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}