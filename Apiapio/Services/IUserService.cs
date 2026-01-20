using Apiapio.Models;

namespace Apiapio.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(UserDto user);
        Task<UserDto?> UpdateUserAsync(int id, UserDto user);
        Task<bool> DeleteUserAsync(int id);
    }
}
