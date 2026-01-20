using Apiapio.Models;

namespace Apiapio.Repositories
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoDto>> GetAllAsync();
        Task<PhotoDto?> GetByIdAsync(int id);
        Task<PhotoDto> CreateAsync(PhotoDto photo);
        Task<PhotoDto?> UpdateAsync(int id, PhotoDto photo);
        Task<bool> DeleteAsync(int id);
    }
}
