namespace OrderDtos
{
    public class OrderExecutedDto
    {
        public string UserId { get; set; }

        public string Ticker { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Side { get; set; }
    }
}