using IPTV.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IPTV.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IPTVDbContext>();

        // Ensure database is created
        await context.Database.MigrateAsync();

        // Check if data already exists
        if (await context.Users.AnyAsync() || await context.Contents.AnyAsync())
        {
            return; // Database has been seeded
        }

        // Seed Users
        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@iptv.com",
            PasswordHash = HashPassword("Admin@123"), // In production, use the AuthService to hash
            FullName = "System Administrator",
            IsActive = true,
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };

        var testUser = new User
        {
            Username = "testuser",
            Email = "test@iptv.com",
            PasswordHash = HashPassword("Test@123"),
            FullName = "Test User",
            IsActive = true,
            Role = UserRole.Premium,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddRangeAsync(adminUser, testUser);
        await context.SaveChangesAsync();

        // Seed Content
        var contents = new List<Content>
        {
            new Content
            {
                Title = "Breaking News Live",
                Description = "24/7 breaking news coverage from around the world",
                StreamUrl = "https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8",
                ThumbnailUrl = "https://picsum.photos/400/225?random=1",
                Type = ContentType.LiveTV,
                Genre = "News",
                Rating = 4.5,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Content
            {
                Title = "Sports World",
                Description = "Live sports coverage and highlights",
                StreamUrl = "https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8",
                ThumbnailUrl = "https://picsum.photos/400/225?random=2",
                Type = ContentType.LiveTV,
                Genre = "Sports",
                Rating = 4.8,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Content
            {
                Title = "The Amazing Adventure",
                Description = "An epic adventure movie with stunning visuals",
                StreamUrl = "https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8",
                ThumbnailUrl = "https://picsum.photos/400/225?random=3",
                Type = ContentType.Movie,
                Duration = 7200,
                ReleaseDate = new DateTime(2023, 6, 15),
                Genre = "Adventure",
                Rating = 4.6,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Content
            {
                Title = "Mystery Mansion",
                Description = "A thrilling mystery series",
                StreamUrl = "https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8",
                ThumbnailUrl = "https://picsum.photos/400/225?random=4",
                Type = ContentType.Series,
                Duration = 2400,
                ReleaseDate = new DateTime(2023, 9, 1),
                Genre = "Mystery",
                Rating = 4.3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Content
            {
                Title = "Comedy Central Live",
                Description = "Stand-up comedy and comedy shows",
                StreamUrl = "https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8",
                ThumbnailUrl = "https://picsum.photos/400/225?random=5",
                Type = ContentType.VOD,
                Duration = 3600,
                Genre = "Comedy",
                Rating = 4.7,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Contents.AddRangeAsync(contents);
        await context.SaveChangesAsync();

        // Seed Channels
        var channels = new List<Channel>
        {
            new Channel
            {
                Name = "CNN International",
                StreamUrl = "https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8",
                LogoUrl = "https://picsum.photos/100/100?random=11",
                ChannelNumber = 1,
                Category = "News",
                Language = "English",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Channel
            {
                Name = "ESPN Sports",
                StreamUrl = "https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8",
                LogoUrl = "https://picsum.photos/100/100?random=12",
                ChannelNumber = 2,
                Category = "Sports",
                Language = "English",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Channel
            {
                Name = "Discovery Channel",
                StreamUrl = "https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8",
                LogoUrl = "https://picsum.photos/100/100?random=13",
                ChannelNumber = 3,
                Category = "Documentary",
                Language = "English",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Channel
            {
                Name = "HBO Entertainment",
                StreamUrl = "https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8",
                LogoUrl = "https://picsum.photos/100/100?random=14",
                ChannelNumber = 4,
                Category = "Entertainment",
                Language = "English",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Channel
            {
                Name = "Kids Network",
                StreamUrl = "https://demo.unified-streaming.com/k8s/features/stable/video/tears-of-steel/tears-of-steel.ism/.m3u8",
                LogoUrl = "https://picsum.photos/100/100?random=15",
                ChannelNumber = 5,
                Category = "Kids",
                Language = "English",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Channels.AddRangeAsync(channels);
        await context.SaveChangesAsync();

        // Seed EPG Programs
        var now = DateTime.UtcNow;
        var epgPrograms = new List<EPGProgram>();

        foreach (var channel in channels)
        {
            // Create programs for the next 24 hours
            for (int i = 0; i < 24; i++)
            {
                epgPrograms.Add(new EPGProgram
                {
                    ChannelId = channel.Id,
                    Title = $"{channel.Name} Program {i + 1}",
                    Description = $"Description for program {i + 1} on {channel.Name}",
                    StartTime = now.AddHours(i),
                    EndTime = now.AddHours(i + 1),
                    Category = channel.Category,
                    Rating = "PG",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await context.EPGPrograms.AddRangeAsync(epgPrograms);
        await context.SaveChangesAsync();

        // Seed User Preferences
        var preferences = new List<UserPreference>
        {
            new UserPreference
            {
                UserId = testUser.Id,
                FavoriteGenres = "Action,Adventure,Comedy",
                FavoriteChannels = "1,2,4",
                Language = "en",
                EnableNotifications = true,
                AutoPlayNext = true,
                PreferredQuality = 1080,
                SubtitlesEnabled = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.UserPreferences.AddRangeAsync(preferences);
        await context.SaveChangesAsync();
    }

    // Password hashing for seed data - matches AuthService implementation
    private static string HashPassword(string password)
    {
        // Using PBKDF2 with HMAC-SHA256 - same as AuthService
        const int iterations = 10000;
        const int hashSize = 32; // 256 bits
        const int saltSize = 16; // 128 bits

        // Generate a random salt
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var salt = new byte[saltSize];
        rng.GetBytes(salt);

        // Hash the password
        using var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            System.Security.Cryptography.HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(hashSize);

        // Combine salt and hash
        var hashBytes = new byte[saltSize + hashSize];
        Array.Copy(salt, 0, hashBytes, 0, saltSize);
        Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

        // Convert to base64 for storage
        return Convert.ToBase64String(hashBytes);
    }
}
