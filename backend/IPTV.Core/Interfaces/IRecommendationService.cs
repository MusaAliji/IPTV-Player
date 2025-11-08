using IPTV.Core.Entities;

namespace IPTV.Core.Interfaces;

public interface IRecommendationService
{
    Task<IEnumerable<Content>> GetRecommendedContentAsync(int userId, int count = 10);
    Task<IEnumerable<Content>> GetSimilarContentAsync(int contentId, int count = 10);
    Task<IEnumerable<Content>> GetRecommendedByGenreAsync(int userId, string genre, int count = 10);
    Task<IEnumerable<Channel>> GetRecommendedChannelsAsync(int userId, int count = 10);
}
