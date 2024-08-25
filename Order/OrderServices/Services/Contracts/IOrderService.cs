using OrderDtos;

namespace OrderServices.Services.Contracts
{
    public interface IOrderService
    {
        Task<OrderDto> AddOrderAsync(int userId, CreateOrderDto createOrderDTO);
    }
}