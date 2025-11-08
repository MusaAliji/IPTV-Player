using IPTV.Core.Entities;

namespace IPTV.Core.Interfaces;

public interface IAuthService
{
    Task<User> RegisterAsync(string username, string email, string password, string? fullName = null);
    Task<(User user, string token)?> LoginAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task UpdateUserAsync(User user);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<UserPreference?> GetUserPreferencesAsync(int userId);
    Task UpdateUserPreferencesAsync(UserPreference preferences);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    string GenerateJwtToken(User user);
}
