using Apiapio.Services;
using Microsoft.IdentityModel.Tokens;
using Apiapio.Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Apiapio.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly InMemoryUserStore _userStore;

        public AuthService(
            IConfiguration configuration,
            ILogger<AuthService> logger,
            InMemoryUserStore userStore)
        {
            _configuration = configuration;
            _logger = logger;
            _userStore = userStore;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userStore.GetByUsernameAsync(request.Username);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                return null;
            }

            var token = GenerateJwtToken(user);
            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 60);

            _logger.LogInformation("User {Username} logged in successfully", user.Username);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userStore.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Username {Username} already exists", request.Username);
                return null;
            }

            var existingEmail = await _userStore.GetByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists", request.Email);
                return null;
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                Role = "User"
            };

            await _userStore.AddAsync(user);

            var token = GenerateJwtToken(user);
            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 60);

            _logger.LogInformation("User {Username} registered successfully", user.Username);

            return new AuthResponse
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
            };
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userStore.GetByUsernameAsync(username);
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] 
                ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expirationMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationMinutes", 60);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            var hash = HashPassword(password);
            return hash == passwordHash;
        }
    }
}
