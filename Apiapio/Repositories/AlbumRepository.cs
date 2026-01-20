using Apiapio.Models;
using Apiapio.Services;
using System.Text;
using System.Text.Json;

namespace Apiapio.Repositories
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AlbumRepository> _logger;
        private readonly InMemoryAlbumCache _cache;

        public AlbumRepository(
            HttpClient httpClient,
            ILogger<AlbumRepository> logger,
            InMemoryAlbumCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<AlbumDto>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("albums");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var albums = JsonSerializer.Deserialize<List<AlbumDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AlbumDto>();

                var cachedAlbums = _cache.GetAll();
                var allAlbums = albums.Concat(cachedAlbums).ToList();

                _logger.LogInformation("Retrieved {ExternalCount} albums from API and {CachedCount} from cache",
                    albums.Count, cachedAlbums.Count());

                return allAlbums;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error fetching albums from external API");
                throw;
            }
        }

        public async Task<AlbumDto?> GetByIdAsync(int id)
        {
            var cachedAlbum = _cache.Get(id);
            if (cachedAlbum != null)
            {
                return cachedAlbum;
            }

            var albums = await GetAllAsync();
            return albums.FirstOrDefault(a => a.Id == id);
        }

        public async Task<IEnumerable<AlbumDto>> GetByUserIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"users/{userId}/albums");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var albums = JsonSerializer.Deserialize<List<AlbumDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AlbumDto>();

                var cachedAlbums = _cache.GetByUserId(userId);
                var allAlbums = albums.Concat(cachedAlbums).ToList();

                return allAlbums;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error fetching albums for user {UserId}", userId);
                throw;
            }
        }

        public async Task<AlbumDto> CreateAsync(AlbumDto album)
        {
            _cache.Add(album);
            _logger.LogInformation("Album created locally with ID: {Id}", album.Id);

            try
            {
                var jsonContent = JsonSerializer.Serialize(album);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync("albums", content);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not sync to external API, but album saved locally");
            }

            return album;
        }

        public async Task<AlbumDto?> UpdateAsync(int id, AlbumDto album)
        {
            if (_cache.Update(id, album))
            {
                _logger.LogInformation("Album {Id} updated in cache", id);
                return album;
            }

            try
            {
                album.Id = id;
                var jsonContent = JsonSerializer.Serialize(album);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"albums/{id}", content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Album {Id} update simulated (external API)", id);
                return album;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error updating album {Id}", id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_cache.Delete(id))
            {
                _logger.LogInformation("Album {Id} deleted from cache", id);
                return true;
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"albums/{id}");
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Album {Id} deletion simulated (external API)", id);
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error deleting album {Id}", id);
                return false;
            }
        }
    }
}
