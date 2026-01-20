using Apiapio.Models.Auth;

namespace Apiapio.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
