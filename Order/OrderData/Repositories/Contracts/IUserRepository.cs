using OrderData.Models;

namespace OrderData.Repositories.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int userId);

        Task AddUserAsync(User user);
    }
}
