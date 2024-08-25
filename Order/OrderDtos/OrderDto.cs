namespace OrderDtos
{
    public class OrderDto
    {
        public int Id { get; set; }

        public string Ticker { get; set; }

        public int Quantity { get; set; }

        public string Side { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}