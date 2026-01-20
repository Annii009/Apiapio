using Apiapio.Models;

namespace Apiapio.Services
{
    public interface IAlbumService
    {
        Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync();
        Task<AlbumDto?> GetAlbumByIdAsync(int id);
        Task<IEnumerable<AlbumDto>> GetAlbumsByUserIdAsync(int userId);
        Task<AlbumDto> CreateAlbumAsync(AlbumDto album);
        Task<AlbumDto?> UpdateAlbumAsync(int id, AlbumDto album);
        Task<bool> DeleteAlbumAsync(int id);
    }
}
