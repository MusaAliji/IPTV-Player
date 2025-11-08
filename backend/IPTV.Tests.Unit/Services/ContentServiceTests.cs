using FluentAssertions;
using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Infrastructure.Services;
using IPTV.Tests.Unit.Helpers;
using Moq;
using Xunit;

namespace IPTV.Tests.Unit.Services;

public class ContentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Content>> _contentRepositoryMock;
    private readonly ContentService _contentService;

    public ContentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _contentRepositoryMock = new Mock<IRepository<Content>>();
        _unitOfWorkMock.Setup(u => u.Contents).Returns(_contentRepositoryMock.Object);
        _contentService = new ContentService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllContentAsync_ShouldReturnAllContent()
    {
        // Arrange
        var testContent = TestDataBuilder.CreateTestContentList(5);
        _contentRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(testContent);

        // Act
        var result = await _contentService.GetAllContentAsync();

        // Assert
        result.Should().HaveCount(5);
        result.Should().BeEquivalentTo(testContent);
        _contentRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetContentByIdAsync_ShouldReturnContent_WhenContentExists()
    {
        // Arrange
        var testContent = TestDataBuilder.CreateTestContent(1);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(testContent);

        // Act
        var result = await _contentService.GetContentByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(testContent);
        _contentRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetContentByIdAsync_ShouldReturnNull_WhenContentDoesNotExist()
    {
        // Arrange
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Content?)null);

        // Act
        var result = await _contentService.GetContentByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetContentByTypeAsync_ShouldReturnFilteredContent()
    {
        // Arrange
        var testContent = new List<Content>
        {
            TestDataBuilder.CreateTestContent(1, "Movie 1", ContentType.Movie),
            TestDataBuilder.CreateTestContent(2, "Movie 2", ContentType.Movie)
        };
        _contentRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Content, bool>>>()))
            .ReturnsAsync(testContent);

        // Act
        var result = await _contentService.GetContentByTypeAsync(ContentType.Movie);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.Type == ContentType.Movie);
    }

    [Fact]
    public async Task GetContentByGenreAsync_ShouldReturnFilteredContent()
    {
        // Arrange
        var testContent = new List<Content>
        {
            TestDataBuilder.CreateTestContent(1, "Action Movie 1"),
            TestDataBuilder.CreateTestContent(2, "Action Movie 2")
        };
        _contentRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Content, bool>>>()))
            .ReturnsAsync(testContent);

        // Act
        var result = await _contentService.GetContentByGenreAsync("Action");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task SearchContentAsync_ShouldReturnMatchingContent()
    {
        // Arrange
        var testContent = new List<Content>
        {
            TestDataBuilder.CreateTestContent(1, "Adventure Quest"),
            TestDataBuilder.CreateTestContent(2, "The Great Adventure"),
            TestDataBuilder.CreateTestContent(3, "Sci-Fi Movie")
        };
        _contentRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(testContent);

        // Act
        var result = await _contentService.SearchContentAsync("Adventure");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.Title.Contains("Adventure", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CreateContentAsync_ShouldCreateAndReturnContent()
    {
        // Arrange
        var newContent = TestDataBuilder.CreateTestContent();
        _contentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Content>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _contentService.CreateContentAsync(newContent);

        // Assert
        result.Should().NotBeNull();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _contentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Content>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateContentAsync_ShouldUpdateContent()
    {
        // Arrange
        var content = TestDataBuilder.CreateTestContent(1);
        content.Title = "Updated Title";
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _contentService.UpdateContentAsync(content);

        // Assert
        content.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _contentRepositoryMock.Verify(r => r.Update(content), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteContentAsync_ShouldDeleteContent_WhenContentExists()
    {
        // Arrange
        var content = TestDataBuilder.CreateTestContent(1);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(content);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _contentService.DeleteContentAsync(1);

        // Assert
        _contentRepositoryMock.Verify(r => r.Remove(content), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteContentAsync_ShouldDoNothing_WhenContentDoesNotExist()
    {
        // Arrange
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Content?)null);

        // Act
        await _contentService.DeleteContentAsync(999);

        // Assert
        _contentRepositoryMock.Verify(r => r.Remove(It.IsAny<Content>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetTrendingContentAsync_ShouldReturnMostRecentContent()
    {
        // Arrange
        var testContent = TestDataBuilder.CreateTestContentList(10);
        _contentRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(testContent);

        // Act
        var result = await _contentService.GetTrendingContentAsync(5);

        // Assert
        result.Should().HaveCountLessOrEqualTo(5);
    }

    [Fact]
    public async Task GetRecentContentAsync_ShouldReturnRecentContent()
    {
        // Arrange
        var testContent = TestDataBuilder.CreateTestContentList(10);
        _contentRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(testContent);

        // Act
        var result = await _contentService.GetRecentContentAsync(3);

        // Assert
        result.Should().HaveCountLessOrEqualTo(3);
    }
}
