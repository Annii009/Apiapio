using Apiapio.Models;

namespace Apiapio.Repositories
{
    public interface IAlbumRepository
    {
        Task<IEnumerable<AlbumDto>> GetAllAsync();
        Task<AlbumDto?> GetByIdAsync(int id);
        Task<IEnumerable<AlbumDto>> GetByUserIdAsync(int userId);
        Task<AlbumDto> CreateAsync(AlbumDto album);
        Task<AlbumDto?> UpdateAsync(int id, AlbumDto album);
        Task<bool> DeleteAsync(int id);
    }
}
