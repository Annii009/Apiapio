using Apiapio.Models;

namespace Apiapio.Services
{
    public interface IPhotoService
    {
        Task<IEnumerable<PhotoDto>> GetAllPhotosAsync();
        Task<PhotoDto?> GetPhotoByIdAsync(int id);
        Task<PhotoDto> CreatePhotoAsync(PhotoDto photo);
        Task<PhotoDto?> UpdatePhotoAsync(int id, PhotoDto photo);
        Task<bool> DeletePhotoAsync(int id);
    }
}