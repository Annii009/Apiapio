using Apiapio.Models;
using Apiapio.Repositories;

namespace Apiapio.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _repository;
        private readonly ILogger<AlbumService> _logger;

        public AlbumService(
            IAlbumRepository repository,
            ILogger<AlbumService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<AlbumDto?> GetAlbumByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid album ID: {Id}", id);
                return null;
            }

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AlbumDto>> GetAlbumsByUserIdAsync(int userId)
        {
            if (userId <= 0)
            {
                _logger.LogWarning("Invalid user ID: {UserId}", userId);
                return Enumerable.Empty<AlbumDto>();
            }

            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task<AlbumDto> CreateAlbumAsync(AlbumDto album)
        {
            if (string.IsNullOrWhiteSpace(album.Title))
            {
                throw new ArgumentException("Title is required");
            }

            if (album.UserId <= 0)
            {
                throw new ArgumentException("Valid UserId is required");
            }

            return await _repository.CreateAsync(album);
        }

        public async Task<AlbumDto?> UpdateAlbumAsync(int id, AlbumDto album)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid album ID for update: {Id}", id);
                return null;
            }

            return await _repository.UpdateAsync(id, album);
        }

        public async Task<bool> DeleteAlbumAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid album ID for deletion: {Id}", id);
                return false;
            }

            return await _repository.DeleteAsync(id);
        }
    }
}
