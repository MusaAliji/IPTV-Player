using IPTV.Core.Entities;
using IPTV.Core.Interfaces;

namespace IPTV.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IUnitOfWork _unitOfWork;

    public AnalyticsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ViewingHistory> RecordViewingAsync(int userId, int? contentId, int? channelId, string? deviceInfo)
    {
        var viewingHistory = new ViewingHistory
        {
            UserId = userId,
            ContentId = contentId,
            ChannelId = channelId,
            StartTime = DateTime.UtcNow,
            DeviceInfo = deviceInfo,
            Completed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ViewingHistories.AddAsync(viewingHistory);
        await _unitOfWork.SaveChangesAsync();
        return viewingHistory;
    }

    public async Task UpdateViewingProgressAsync(int viewingHistoryId, int progress, bool completed)
    {
        var viewingHistory = await _unitOfWork.ViewingHistories.GetByIdAsync(viewingHistoryId);
        if (viewingHistory != null)
        {
            viewingHistory.Progress = progress;
            viewingHistory.Completed = completed;
            if (completed)
            {
                viewingHistory.EndTime = DateTime.UtcNow;
                viewingHistory.Duration = (int)(viewingHistory.EndTime.Value - viewingHistory.StartTime).TotalSeconds;
            }
            _unitOfWork.ViewingHistories.Update(viewingHistory);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ViewingHistory>> GetUserViewingHistoryAsync(int userId, int count = 50)
    {
        var history = await _unitOfWork.ViewingHistories.FindAsync(vh => vh.UserId == userId);
        return history.OrderByDescending(vh => vh.StartTime).Take(count);
    }

    public async Task<ViewingHistory?> GetLastViewingForContentAsync(int userId, int contentId)
    {
        var history = await _unitOfWork.ViewingHistories.FindAsync(vh =>
            vh.UserId == userId && vh.ContentId == contentId
        );
        return history.OrderByDescending(vh => vh.StartTime).FirstOrDefault();
    }

    public async Task<Dictionary<string, int>> GetViewingStatsByGenreAsync(int userId)
    {
        var viewingHistory = await _unitOfWork.ViewingHistories.FindAsync(vh => vh.UserId == userId);
        var contentIds = viewingHistory
            .Where(vh => vh.ContentId.HasValue)
            .Select(vh => vh.ContentId!.Value)
            .Distinct()
            .ToList(); // Materialize to avoid multiple enumerations

        var stats = new Dictionary<string, int>();

        foreach (var contentId in contentIds)
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(contentId);
            if (content?.Genre != null)
            {
                if (stats.ContainsKey(content.Genre))
                    stats[content.Genre]++;
                else
                    stats[content.Genre] = 1;
            }
        }

        return stats;
    }

    public async Task<Dictionary<string, int>> GetMostWatchedContentAsync(int count = 10)
    {
        var allHistory = await _unitOfWork.ViewingHistories.GetAllAsync();
        var contentViews = allHistory
            .Where(vh => vh.ContentId.HasValue)
            .GroupBy(vh => vh.ContentId!.Value)
            .Select(g => new { ContentId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToList(); // Materialize to avoid multiple enumerations

        var result = new Dictionary<string, int>();
        foreach (var item in contentViews)
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(item.ContentId);
            if (content != null)
            {
                result[content.Title] = item.Count;
            }
        }

        return result;
    }

    public async Task<Dictionary<string, int>> GetMostWatchedChannelsAsync(int count = 10)
    {
        var allHistory = await _unitOfWork.ViewingHistories.GetAllAsync();
        var channelViews = allHistory
            .Where(vh => vh.ChannelId.HasValue)
            .GroupBy(vh => vh.ChannelId!.Value)
            .Select(g => new { ChannelId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(count)
            .ToList(); // Materialize to avoid multiple enumerations

        var result = new Dictionary<string, int>();
        foreach (var item in channelViews)
        {
            var channel = await _unitOfWork.Channels.GetByIdAsync(item.ChannelId);
            if (channel != null)
            {
                result[channel.Name] = item.Count;
            }
        }

        return result;
    }

    public async Task<int> GetTotalViewingTimeAsync(int userId)
    {
        var history = await _unitOfWork.ViewingHistories.FindAsync(vh => vh.UserId == userId && vh.Duration.HasValue);
        return history.Sum(vh => vh.Duration ?? 0);
    }

    public async Task<IEnumerable<Content>> GetContinueWatchingAsync(int userId)
    {
        var incompleteHistory = await _unitOfWork.ViewingHistories.FindAsync(vh =>
            vh.UserId == userId &&
            vh.ContentId.HasValue &&
            !vh.Completed &&
            vh.Progress.HasValue &&
            vh.Progress > 0
        );

        var contentIds = incompleteHistory
            .OrderByDescending(vh => vh.StartTime)
            .Select(vh => vh.ContentId!.Value)
            .Distinct()
            .Take(10)
            .ToList(); // Materialize to avoid multiple enumerations

        var contents = new List<Content>();
        foreach (var contentId in contentIds)
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(contentId);
            if (content != null)
            {
                contents.Add(content);
            }
        }

        return contents;
    }
}
