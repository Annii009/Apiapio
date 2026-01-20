using Apiapio.Models;
using System.Text;
using System.Text.Json;

namespace Apiapio.Repositories
{
    public class JsonPlaceholderRepository : IPhotoRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JsonPlaceholderRepository> _logger;

        public JsonPlaceholderRepository(
            HttpClient httpClient,
            ILogger<JsonPlaceholderRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<PhotoDto>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("albums/1/photos");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var photos = JsonSerializer.Deserialize<List<PhotoDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("Retrieved {Count} photos from external API", photos?.Count ?? 0);
                return photos ?? new List<PhotoDto>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error fetching photos from external API");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error");
                throw;
            }
        }

        public async Task<PhotoDto?> GetByIdAsync(int id)
        {
            var photos = await GetAllAsync();
            return photos.FirstOrDefault(p => p.Id == id);
        }

        public async Task<PhotoDto> CreateAsync(PhotoDto photo)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(photo);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("photos", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var createdPhoto = JsonSerializer.Deserialize<PhotoDto>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("Photo created with simulated ID: {Id}", createdPhoto?.Id);

                return createdPhoto ?? photo;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error creating photo");
                throw;
            }
        }

        public async Task<PhotoDto?> UpdateAsync(int id, PhotoDto photo)
        {
            try
            {
                photo.Id = id;

                var jsonContent = JsonSerializer.Serialize(photo);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"photos/{id}", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var updatedPhoto = JsonSerializer.Deserialize<PhotoDto>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _logger.LogInformation("Photo {Id} updated successfully", id);
                return updatedPhoto;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error updating photo {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"photos/{id}");
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Photo {Id} deleted successfully", id);
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error deleting photo {Id}", id);
                return false;
            }
        }
    }

}