using Apiapio.Models;

namespace Apiapio.Services
{
    public interface IPhotoService
    {
        Task<IEnumerable<PhotoDto>> GetAllPhotosAsync();
        Task<PhotoDto?> GetPhotoByIdAsync(int id);
        Task<IEnumerable<PhotoDto>> GetPhotosByAlbumIdAsync(int albumId);
        Task<IEnumerable<PhotoDto>> SearchPhotosByTitleAsync(string query);
    }
}