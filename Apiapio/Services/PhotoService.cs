using Apiapio.Models;
using Apiapio.Repositories;

namespace Apiapio.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoRepository _repository;
        private readonly ILogger<PhotoService> _logger;

        public PhotoService(
            IPhotoRepository repository,
            ILogger<PhotoService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<PhotoDto>> GetAllPhotosAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PhotoDto?> GetPhotoByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid photo ID: {Id}", id);
                return null;
            }

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PhotoDto>> GetPhotosByAlbumIdAsync(int albumId)
        {
            var photos = await _repository.GetAllAsync();
            return photos.Where(p => p.AlbumId == albumId);
        }

        public async Task<IEnumerable<PhotoDto>> SearchPhotosByTitleAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Enumerable.Empty<PhotoDto>();
            }

            var photos = await _repository.GetAllAsync();
            return photos.Where(p => 
                p.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
        }
    }
}
