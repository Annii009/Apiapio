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

        public async Task<PhotoDto> CreatePhotoAsync(PhotoDto photo)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(photo.Title))
            {
                throw new ArgumentException("Title is required");
            }

            if (string.IsNullOrWhiteSpace(photo.Url))
            {
                throw new ArgumentException("URL is required");
            }

            return await _repository.CreateAsync(photo);
        }

        public async Task<PhotoDto?> UpdatePhotoAsync(int id, PhotoDto photo)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid photo ID for update: {Id}", id);
                return null;
            }

            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(photo.Title))
            {
                throw new ArgumentException("Title is required");
            }

            return await _repository.UpdateAsync(id, photo);
        }

        public async Task<bool> DeletePhotoAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid photo ID for deletion: {Id}", id);
                return false;
            }

            return await _repository.DeleteAsync(id);
        }
    }
}
