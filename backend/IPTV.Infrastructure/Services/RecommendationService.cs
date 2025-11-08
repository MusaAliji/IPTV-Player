using IPTV.Core.Entities;
using IPTV.Core.Interfaces;

namespace IPTV.Infrastructure.Services;

public class RecommendationService : IRecommendationService
{
    private readonly IUnitOfWork _unitOfWork;

    public RecommendationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Content>> GetRecommendedContentAsync(int userId, int count = 10)
    {
        // Get user's viewing history to understand preferences
        var viewingHistory = await _unitOfWork.ViewingHistories.FindAsync(vh => vh.UserId == userId);
        var watchedContentIds = viewingHistory
            .Where(vh => vh.ContentId.HasValue)
            .Select(vh => vh.ContentId!.Value)
            .Distinct()
            .ToList();

        // Get genres from watched content
        var preferredGenres = new List<string>();
        foreach (var contentId in watchedContentIds.Take(20))
        {
            var content = await _unitOfWork.Contents.GetByIdAsync(contentId);
            if (content?.Genre != null && !preferredGenres.Contains(content.Genre))
            {
                preferredGenres.Add(content.Genre);
            }
        }

        // Get content from preferred genres that user hasn't watched
        var allContent = await _unitOfWork.Contents.GetAllAsync();
        var recommendations = allContent
            .Where(c => !watchedContentIds.Contains(c.Id) &&
                       c.Genre != null &&
                       preferredGenres.Contains(c.Genre))
            .OrderByDescending(c => c.Rating ?? 0)
            .ThenByDescending(c => c.CreatedAt)
            .Take(count);

        // If not enough recommendations, add popular content
        if (recommendations.Count() < count)
        {
            var additional = allContent
                .Where(c => !watchedContentIds.Contains(c.Id) &&
                           !recommendations.Contains(c))
                .OrderByDescending(c => c.Rating ?? 0)
                .Take(count - recommendations.Count());

            recommendations = recommendations.Concat(additional);
        }

        return recommendations;
    }

    public async Task<IEnumerable<Content>> GetSimilarContentAsync(int contentId, int count = 10)
    {
        var sourceContent = await _unitOfWork.Contents.GetByIdAsync(contentId);
        if (sourceContent == null)
        {
            return Enumerable.Empty<Content>();
        }

        var allContent = await _unitOfWork.Contents.GetAllAsync();
        var similarContent = allContent
            .Where(c => c.Id != contentId &&
                       (c.Genre == sourceContent.Genre || c.Type == sourceContent.Type))
            .OrderByDescending(c => c.Genre == sourceContent.Genre ? 2 : 0)
            .ThenByDescending(c => c.Rating ?? 0)
            .Take(count);

        return similarContent;
    }

    public async Task<IEnumerable<Content>> GetRecommendedByGenreAsync(int userId, string genre, int count = 10)
    {
        var viewingHistory = await _unitOfWork.ViewingHistories.FindAsync(vh => vh.UserId == userId);
        var watchedContentIds = viewingHistory
            .Where(vh => vh.ContentId.HasValue)
            .Select(vh => vh.ContentId!.Value)
            .Distinct()
            .ToList();

        var allContent = await _unitOfWork.Contents.GetAllAsync();
        var recommendations = allContent
            .Where(c => !watchedContentIds.Contains(c.Id) &&
                       c.Genre == genre)
            .OrderByDescending(c => c.Rating ?? 0)
            .ThenByDescending(c => c.CreatedAt)
            .Take(count);

        return recommendations;
    }

    public async Task<IEnumerable<Channel>> GetRecommendedChannelsAsync(int userId, int count = 10)
    {
        // Get user's channel viewing history
        var viewingHistory = await _unitOfWork.ViewingHistories.FindAsync(vh => vh.UserId == userId);
        var watchedChannelIds = viewingHistory
            .Where(vh => vh.ChannelId.HasValue)
            .Select(vh => vh.ChannelId!.Value)
            .Distinct()
            .ToList();

        // Get categories from watched channels
        var preferredCategories = new List<string>();
        foreach (var channelId in watchedChannelIds.Take(20))
        {
            var channel = await _unitOfWork.Channels.GetByIdAsync(channelId);
            if (channel?.Category != null && !preferredCategories.Contains(channel.Category))
            {
                preferredCategories.Add(channel.Category);
            }
        }

        // Get active channels from preferred categories that user hasn't watched much
        var allChannels = await _unitOfWork.Channels.FindAsync(c => c.IsActive);
        var recommendations = allChannels
            .Where(c => !watchedChannelIds.Contains(c.Id) &&
                       c.Category != null &&
                       preferredCategories.Contains(c.Category))
            .OrderBy(c => c.ChannelNumber)
            .Take(count);

        // If not enough recommendations, add popular active channels
        if (recommendations.Count() < count)
        {
            var additional = allChannels
                .Where(c => !watchedChannelIds.Contains(c.Id) &&
                           !recommendations.Contains(c))
                .OrderBy(c => c.ChannelNumber)
                .Take(count - recommendations.Count());

            recommendations = recommendations.Concat(additional);
        }

        return recommendations;
    }
}
