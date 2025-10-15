using api.Application.DTOs;
using api.Application.Interfaces;
using api.Domain.Entities;
using log4net;
using System.Security.Cryptography;
using System.Text;

namespace api.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IConfiguration configuration) : IAuthService
{
    private static readonly ILog _log = LogManager.GetLogger(typeof(AuthService));
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        _log.Info($"Login attempt for user: {request.Username}");

        User? user = await _userRepository.GetByUsernameAsync(request.Username);
        
        if (user == null)
        {
            _log.Warn($"User not found: {request.Username}");
            return null;
        }

        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            _log.Warn($"Invalid password for user: {request.Username}");
            return null;
        }

        string token = _jwtTokenGenerator.GenerateToken(user);
        int expirationMinutes = int.Parse(_configuration.GetSection("JwtSettings")["ExpirationMinutes"] ?? "60");

        _log.Info($"User logged in successfully: {request.Username}");

        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        _log.Info($"Registration attempt for user: {request.Username}");

        if (await _userRepository.UserExistsAsync(request.Username))
        {
            _log.Warn($"User already exists: {request.Username}");
            throw new InvalidOperationException("Username already exists");
        }

        User user = new()
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        user = await _userRepository.CreateAsync(user);

        string token = _jwtTokenGenerator.GenerateToken(user);
        int expirationMinutes = int.Parse(_configuration.GetSection("JwtSettings")["ExpirationMinutes"] ?? "60");

        _log.Info($"User registered successfully: {request.Username}");

        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await _userRepository.UserExistsAsync(username);
    }

    private static string HashPassword(string password)
    {
        byte[] hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string passwordHash) => HashPassword(password) == passwordHash;
}
