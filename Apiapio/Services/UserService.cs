using Apiapio.Models;
using Apiapio.Repositories;

namespace Apiapio.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository repository,
            ILogger<UserService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID: {Id}", id);
                return null;
            }

            return await _repository.GetByIdAsync(id);
        }

        public async Task<UserDto> CreateUserAsync(UserDto user)
        {
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                throw new ArgumentException("Name is required");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Email is required");
            }

            return await _repository.CreateAsync(user);
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UserDto user)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID for update: {Id}", id);
                return null;
            }

            return await _repository.UpdateAsync(id, user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID for deletion: {Id}", id);
                return false;
            }

            return await _repository.DeleteAsync(id);
        }
    }
}
