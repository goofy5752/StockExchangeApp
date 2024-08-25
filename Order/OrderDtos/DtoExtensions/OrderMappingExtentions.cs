using OrderData.Models;

namespace OrderDtos.DtoExtensions
{
    public static class OrderMappingExtentions
    {
        public static OrderDto ToOrderDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                Ticker = order.Ticker,
                Quantity = order.Quantity,
                Side = order.Side,
                Price = order.Price,
                CreatedOn = order.CreatedOn,
            };
        }

        public static Order ToOrder(this CreateOrderDto createOrderDto, int userId, decimal price)
        {
            return new Order
            {
                Ticker = createOrderDto.Ticker,
                Quantity = createOrderDto.Quantity,
                Side = createOrderDto.Side,
                Price = price,
                CreatedOn = DateTime.UtcNow,
                UserId = userId,
            };
        }

        public static OrderExecutedDto ToOrderExecutedDto(this Order order)
        {
            return new OrderExecutedDto
            {
                UserId = order.UserId.ToString(),
                Ticker = order.Ticker,
                Quantity = order.Quantity,
                Price = order.Price,
                Side = order.Side
            };
        }
    }
}
