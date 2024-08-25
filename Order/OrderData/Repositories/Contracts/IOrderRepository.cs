using OrderData.Models;

namespace OrderData.Repositories.Contracts
{
    public interface IOrderRepository
    {
        Task<Order> AddOrderAsync(Order order);
    }
}
