using Apiapio.Models;

namespace Apiapio.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto> CreateAsync(UserDto user);
        Task<UserDto?> UpdateAsync(int id, UserDto user);
        Task<bool> DeleteAsync(int id);
    }
}