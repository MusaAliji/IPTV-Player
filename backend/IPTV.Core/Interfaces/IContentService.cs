using IPTV.Core.Entities;

namespace IPTV.Core.Interfaces;

public interface IContentService
{
    Task<IEnumerable<Content>> GetAllContentAsync();
    Task<Content?> GetContentByIdAsync(int id);
    Task<IEnumerable<Content>> GetContentByTypeAsync(ContentType type);
    Task<IEnumerable<Content>> GetContentByGenreAsync(string genre);
    Task<IEnumerable<Content>> SearchContentAsync(string searchTerm);
    Task<Content> CreateContentAsync(Content content);
    Task UpdateContentAsync(Content content);
    Task DeleteContentAsync(int id);
    Task<IEnumerable<Content>> GetTrendingContentAsync(int count);
    Task<IEnumerable<Content>> GetRecentContentAsync(int count);
}
