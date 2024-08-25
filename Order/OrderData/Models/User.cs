using OrderData.Common;

namespace OrderData.Models
{
    public class User : BaseModel<int>
    {
        public List<Order> Orders { get; set; }
    }
}