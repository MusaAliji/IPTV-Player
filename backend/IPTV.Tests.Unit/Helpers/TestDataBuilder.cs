using IPTV.Core.Entities;

namespace IPTV.Tests.Unit.Helpers;

public static class TestDataBuilder
{
    public static User CreateTestUser(int id = 1, string username = "testuser", UserRole role = UserRole.User)
    {
        return new User
        {
            Id = id,
            Username = username,
            Email = $"{username}@test.com",
            PasswordHash = "hashedpassword",
            FullName = "Test User",
            IsActive = true,
            Role = role,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };
    }

    public static Content CreateTestContent(int id = 1, string title = "Test Content", ContentType type = ContentType.Movie)
    {
        return new Content
        {
            Id = id,
            Title = title,
            Description = "Test Description",
            StreamUrl = "https://test.com/stream.m3u8",
            ThumbnailUrl = "https://test.com/thumb.jpg",
            Type = type,
            Duration = 7200,
            ReleaseDate = DateTime.UtcNow.AddDays(-30),
            Genre = "Action",
            Rating = 4.5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Channel CreateTestChannel(int id = 1, string name = "Test Channel")
    {
        return CreateTestChannel(id, name, true);
    }

    public static Channel CreateTestChannel(int id, bool isActive)
    {
        return CreateTestChannel(id, $"Test Channel {id}", isActive);
    }

    public static Channel CreateTestChannel(int id, string name, bool isActive)
    {
        return new Channel
        {
            Id = id,
            Name = name,
            StreamUrl = "https://test.com/channel.m3u8",
            LogoUrl = "https://test.com/logo.png",
            ChannelNumber = id,
            Category = "News",
            Language = "English",
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static EPGProgram CreateTestEPGProgram(int id = 1, int channelId = 1)
    {
        return CreateTestEPGProgram(id, channelId, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
    }

    public static EPGProgram CreateTestEPGProgram(int id, int channelId, DateTime startTime, DateTime endTime)
    {
        return new EPGProgram
        {
            Id = id,
            ChannelId = channelId,
            Title = $"Test Program {id}",
            Description = "Test Program Description",
            StartTime = startTime,
            EndTime = endTime,
            Category = "Entertainment",
            Rating = "PG",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static ViewingHistory CreateTestViewingHistory(int id = 1, int userId = 1, int? contentId = 1)
    {
        return new ViewingHistory
        {
            Id = id,
            UserId = userId,
            ContentId = contentId,
            ChannelId = null,
            StartTime = DateTime.UtcNow.AddHours(-2),
            EndTime = DateTime.UtcNow,
            Duration = 7200,
            Progress = 3600,
            Completed = false,
            DeviceInfo = "Test Device",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static UserPreference CreateTestUserPreference(int id = 1, int userId = 1)
    {
        return new UserPreference
        {
            Id = id,
            UserId = userId,
            FavoriteGenres = "Action,Comedy",
            FavoriteChannels = "1,2,3",
            Language = "en",
            EnableNotifications = true,
            AutoPlayNext = true,
            PreferredQuality = 1080,
            SubtitlesEnabled = false,
            SubtitleLanguage = "en",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static List<Content> CreateTestContentList(int count = 5)
    {
        var contents = new List<Content>();
        for (int i = 1; i <= count; i++)
        {
            contents.Add(CreateTestContent(i, $"Content {i}", (ContentType)(i % 4)));
        }
        return contents;
    }

    public static List<Channel> CreateTestChannelList(int count = 5)
    {
        var channels = new List<Channel>();
        for (int i = 1; i <= count; i++)
        {
            channels.Add(CreateTestChannel(i, $"Channel {i}"));
        }
        return channels;
    }

    public static List<EPGProgram> CreateTestEPGProgramList(int count = 5, int channelId = 1)
    {
        var programs = new List<EPGProgram>();
        for (int i = 1; i <= count; i++)
        {
            var startTime = DateTime.UtcNow.AddHours(i);
            var endTime = startTime.AddHours(1);
            programs.Add(CreateTestEPGProgram(i, channelId, startTime, endTime));
        }
        return programs;
    }
}
