namespace OrderDtos
{
    public class StockPriceDto
    {
        public string Ticker { get; set; }

        public decimal Price { get; set; }

        public DateTime Timestamp { get; set; }
    }
}