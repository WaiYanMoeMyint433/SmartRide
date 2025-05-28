// Services/IUserService.cs
using SmartRide.Models;
using SmartRide.ViewModels;

namespace SmartRide.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterViewModel model);
        Task<User> LoginAsync(string email, string password);
        Task<User> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
    }
}