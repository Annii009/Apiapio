using Apiapio.Models.Auth;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace Apiapio.Services
{
    public class InMemoryUserStore
    {
        private readonly ConcurrentDictionary<int, User> _users = new();
        private int _nextId = 1;

        public InMemoryUserStore()
        {
            // Crear usuario admin por defecto para testing
            var adminPasswordHash = HashPassword("Admin123!");
            var adminUser = new User
            {
                Id = _nextId++,
                Username = "admin",
                Email = "admin@photosgateway.com",
                PasswordHash = adminPasswordHash,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };
            _users.TryAdd(adminUser.Id, adminUser);
        }

        public Task AddAsync(User user)
        {
            user.Id = _nextId++;
            _users.TryAdd(user.Id, user);
            return Task.CompletedTask;
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            var user = _users.Values.FirstOrDefault(u => 
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            var user = _users.Values.FirstOrDefault(u => 
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            _users.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
