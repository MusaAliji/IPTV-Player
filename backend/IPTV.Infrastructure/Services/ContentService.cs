using IPTV.Core.Entities;
using IPTV.Core.Interfaces;

namespace IPTV.Infrastructure.Services;

public class ContentService : IContentService
{
    private readonly IUnitOfWork _unitOfWork;

    public ContentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Content>> GetAllContentAsync()
    {
        return await _unitOfWork.Contents.GetAllAsync();
    }

    public async Task<Content?> GetContentByIdAsync(int id)
    {
        return await _unitOfWork.Contents.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Content>> GetContentByTypeAsync(ContentType type)
    {
        return await _unitOfWork.Contents.FindAsync(c => c.Type == type);
    }

    public async Task<IEnumerable<Content>> GetContentByGenreAsync(string genre)
    {
        return await _unitOfWork.Contents.FindAsync(c => c.Genre == genre);
    }

    public async Task<IEnumerable<Content>> SearchContentAsync(string searchTerm)
    {
        var allContent = await _unitOfWork.Contents.GetAllAsync();
        return allContent.Where(c =>
            c.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            (c.Description != null && c.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        );
    }

    public async Task<Content> CreateContentAsync(Content content)
    {
        content.CreatedAt = DateTime.UtcNow;
        content.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Contents.AddAsync(content);
        await _unitOfWork.SaveChangesAsync();
        return content;
    }

    public async Task UpdateContentAsync(Content content)
    {
        content.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Contents.Update(content);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteContentAsync(int id)
    {
        var content = await _unitOfWork.Contents.GetByIdAsync(id);
        if (content != null)
        {
            _unitOfWork.Contents.Remove(content);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Content>> GetTrendingContentAsync(int count)
    {
        // For now, return most recent content. In future, implement based on viewing stats
        var allContent = await _unitOfWork.Contents.GetAllAsync();
        return allContent
            .OrderByDescending(c => c.CreatedAt)
            .Take(count);
    }

    public async Task<IEnumerable<Content>> GetRecentContentAsync(int count)
    {
        var allContent = await _unitOfWork.Contents.GetAllAsync();
        return allContent
            .OrderByDescending(c => c.CreatedAt)
            .Take(count);
    }
}
