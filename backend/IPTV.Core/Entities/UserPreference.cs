namespace IPTV.Core.Entities;

public class UserPreference
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? FavoriteGenres { get; set; } // Comma-separated genres
    public string? FavoriteChannels { get; set; } // Comma-separated channel IDs
    public string? Language { get; set; }
    public bool EnableNotifications { get; set; }
    public bool AutoPlayNext { get; set; }
    public int? PreferredQuality { get; set; } // 480, 720, 1080, etc.
    public bool SubtitlesEnabled { get; set; }
    public string? SubtitleLanguage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public User? User { get; set; }
}
