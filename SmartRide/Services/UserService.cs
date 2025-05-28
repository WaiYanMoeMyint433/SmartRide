// Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using SmartRide.Data;
using SmartRide.Models;
using SmartRide.ViewModels;

namespace SmartRide.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(RegisterViewModel model)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                throw new Exception("Email already exists");

            User user;

            if (model.UserType == UserType.Driver)
            {
                user = new Driver
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = model.Password, // In production: hash this
                    VehicleType = model.VehicleType.Value,
                    VehicleId = model.VehicleId,
                    LicenseNumber = model.LicenseNumber,
                    Status = DriverStatus.Offline,
                    CreateDate = DateTime.Now
                };
            }
            else
            {
                user = new Customer
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Password = model.Password, // In production: hash this
                    CreateDate = DateTime.Now
                };
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            // In production: compare hashed passwords
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}