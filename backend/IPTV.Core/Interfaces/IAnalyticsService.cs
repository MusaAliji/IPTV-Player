using IPTV.Core.Entities;

namespace IPTV.Core.Interfaces;

public interface IAnalyticsService
{
    Task<ViewingHistory> RecordViewingAsync(int userId, int? contentId, int? channelId, string? deviceInfo);
    Task UpdateViewingProgressAsync(int viewingHistoryId, int progress, bool completed);
    Task<IEnumerable<ViewingHistory>> GetUserViewingHistoryAsync(int userId, int count = 50);
    Task<ViewingHistory?> GetLastViewingForContentAsync(int userId, int contentId);
    Task<Dictionary<string, int>> GetViewingStatsByGenreAsync(int userId);
    Task<Dictionary<string, int>> GetMostWatchedContentAsync(int count = 10);
    Task<Dictionary<string, int>> GetMostWatchedChannelsAsync(int count = 10);
    Task<int> GetTotalViewingTimeAsync(int userId);
    Task<IEnumerable<Content>> GetContinueWatchingAsync(int userId);
}
