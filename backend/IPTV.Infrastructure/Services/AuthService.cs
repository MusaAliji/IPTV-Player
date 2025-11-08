using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IPTV.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<User> RegisterAsync(string username, string email, string password, string? fullName = null)
    {
        // Check if user already exists
        var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u =>
            u.Username == username || u.Email == email
        );

        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this username or email already exists");
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = HashPassword(password),
            FullName = fullName,
            IsActive = true,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Create default preferences for the user
        var preferences = new UserPreference
        {
            UserId = user.Id,
            EnableNotifications = true,
            AutoPlayNext = false,
            SubtitlesEnabled = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.UserPreferences.AddAsync(preferences);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task<(User user, string token)?> LoginAsync(string username, string password)
    {
        var user = await _unitOfWork.Users.FirstOrDefaultAsync(u =>
            u.Username == username || u.Email == username
        );

        if (user == null || !user.IsActive)
        {
            return null;
        }

        if (!VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        return (user, token);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _unitOfWork.Users.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task UpdateUserAsync(User user)
    {
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        if (!VerifyPassword(currentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = HashPassword(newPassword);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<UserPreference?> GetUserPreferencesAsync(int userId)
    {
        return await _unitOfWork.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task UpdateUserPreferencesAsync(UserPreference preferences)
    {
        preferences.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.UserPreferences.Update(preferences);
        await _unitOfWork.SaveChangesAsync();
    }

    public string HashPassword(string password)
    {
        // Using PBKDF2 with HMAC-SHA256
        const int iterations = 10000;
        const int hashSize = 32; // 256 bits
        const int saltSize = 16; // 128 bits

        // Generate a random salt
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[saltSize];
        rng.GetBytes(salt);

        // Hash the password
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(hashSize);

        // Combine salt and hash
        var hashBytes = new byte[saltSize + hashSize];
        Array.Copy(salt, 0, hashBytes, 0, saltSize);
        Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

        // Convert to base64 for storage
        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        const int iterations = 10000;
        const int hashSize = 32;
        const int saltSize = 16;

        try
        {
            // Extract the bytes
            var hashBytes = Convert.FromBase64String(passwordHash);

            // Extract the salt
            var salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            // Compute the hash on the password the user entered
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(hashSize);

            // Compare the results
            for (int i = 0; i < hashSize; i++)
            {
                if (hashBytes[i + saltSize] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "IPTV-Player-Super-Secret-Key-Change-In-Production-Min-32-Chars";
        var issuer = jwtSettings["Issuer"] ?? "IPTVPlayer";
        var audience = jwtSettings["Audience"] ?? "IPTVPlayerClients";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "1440"); // 24 hours default

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
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
}
