using Apiapio.Models;

namespace Apiapio.Repositories
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoDto>> GetAllAsync();
        Task<PhotoDto?> GetByIdAsync(int id);
    }
}
