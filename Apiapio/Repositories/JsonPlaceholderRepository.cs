using Apiapio.Models;
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
    }
}
