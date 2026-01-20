using Apiapio.Models;
using Apiapio.Services;
using System.Text;
using System.Text.Json;

namespace Apiapio.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserRepository> _logger;
        private readonly InMemoryUserCache _cache;

        public UserRepository(
            HttpClient httpClient,
            ILogger<UserRepository> logger,
            InMemoryUserCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("users");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<UserDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<UserDto>();

                var cachedUsers = _cache.GetAll();
                var allUsers = users.Concat(cachedUsers).ToList();

                _logger.LogInformation("Retrieved {ExternalCount} users from API and {CachedCount} from cache",
                    users.Count, cachedUsers.Count());

                return allUsers;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error fetching users from external API");
                throw;
            }
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var cachedUser = _cache.Get(id);
            if (cachedUser != null)
            {
                return cachedUser;
            }

            var users = await GetAllAsync();
            return users.FirstOrDefault(u => u.Id == id);
        }

        public async Task<UserDto> CreateAsync(UserDto user)
        {
            _cache.Add(user);
            _logger.LogInformation("User created locally with ID: {Id}", user.Id);

            try
            {
                var jsonContent = JsonSerializer.Serialize(user);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync("users", content);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not sync to external API, but user saved locally");
            }

            return user;
        }

        public async Task<UserDto?> UpdateAsync(int id, UserDto user)
        {
            if (_cache.Update(id, user))
            {
                _logger.LogInformation("User {Id} updated in cache", id);
                return user;
            }

            try
            {
                user.Id = id;
                var jsonContent = JsonSerializer.Serialize(user);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"users/{id}", content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("User {Id} update simulated (external API)", id);
                return user;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error updating user {Id}", id);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (_cache.Delete(id))
            {
                _logger.LogInformation("User {Id} deleted from cache", id);
                return true;
            }

            try
            {
                var response = await _httpClient.DeleteAsync($"users/{id}");
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("User {Id} deletion simulated (external API)", id);
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error deleting user {Id}", id);
                return false;
            }
        }
    }
}
