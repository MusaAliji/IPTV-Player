using FluentAssertions;
using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Infrastructure.Services;
using IPTV.Tests.Unit.Helpers;
using Moq;
using Xunit;

namespace IPTV.Tests.Unit.Services;

public class RecommendationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<ViewingHistory>> _viewingHistoryRepositoryMock;
    private readonly Mock<IRepository<Content>> _contentRepositoryMock;
    private readonly Mock<IRepository<Channel>> _channelRepositoryMock;
    private readonly RecommendationService _recommendationService;

    public RecommendationServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _viewingHistoryRepositoryMock = new Mock<IRepository<ViewingHistory>>();
        _contentRepositoryMock = new Mock<IRepository<Content>>();
        _channelRepositoryMock = new Mock<IRepository<Channel>>();

        _unitOfWorkMock.Setup(u => u.ViewingHistories).Returns(_viewingHistoryRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Contents).Returns(_contentRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Channels).Returns(_channelRepositoryMock.Object);

        _recommendationService = new RecommendationService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetRecommendedContentAsync_WithViewingHistory_ShouldReturnRecommendedContent()
    {
        // Arrange
        const int userId = 1;
        const int count = 5;

        // User has watched Action content
        var viewingHistory = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, userId, 1)
        };

        var watchedContent = TestDataBuilder.CreateTestContent(1, "Action Movie 1", ContentType.Movie);
        watchedContent.Genre = "Action";

        // Available content - some Action (preferred), some other genres
        var allContent = new List<Content>
        {
            TestDataBuilder.CreateTestContent(1, "Action Movie 1", ContentType.Movie), // Already watched
            TestDataBuilder.CreateTestContent(2, "Action Movie 2", ContentType.Movie),
            TestDataBuilder.CreateTestContent(3, "Action Movie 3", ContentType.Movie),
            TestDataBuilder.CreateTestContent(4, "Comedy Movie", ContentType.Movie),
            TestDataBuilder.CreateTestContent(5, "Drama Series", ContentType.Series)
        };
        allContent[1].Genre = "Action";
        allContent[1].Rating = 4.5;
        allContent[2].Genre = "Action";
        allContent[2].Rating = 4.8;
        allContent[3].Genre = "Comedy";
        allContent[4].Genre = "Drama";

        _viewingHistoryRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(viewingHistory);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(watchedContent);
        _contentRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(allContent);

        // Act
        var result = await _recommendationService.GetRecommendedContentAsync(userId, count);

        // Assert
        var recommendations = result.ToList();
        recommendations.Should().NotBeEmpty();
        recommendations.Should().NotContain(c => c.Id == 1); // Should not include already watched content
        recommendations.Should().Contain(c => c.Genre == "Action"); // Should prefer Action genre
    }

    [Fact]
    public async Task GetRecommendedContentAsync_StackOverflowFix_ShouldNotThrowStackOverflow()
    {
        // This test specifically verifies the StackOverflow fix at line 45 and 50
        // The fix: materializing queries with .ToList() before using them in Contains()

        // Arrange
        const int userId = 1;
        const int count = 10;

        var viewingHistory = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, userId, 1)
        };

        var watchedContent = TestDataBuilder.CreateTestContent(1, "Action Movie", ContentType.Movie);
        watchedContent.Genre = "Action";

        // Create just enough content to trigger the "not enough recommendations" branch
        var allContent = new List<Content>
        {
            TestDataBuilder.CreateTestContent(1, "Watched", ContentType.Movie), // Watched
            TestDataBuilder.CreateTestContent(2, "Recommend 1", ContentType.Movie),
            TestDataBuilder.CreateTestContent(3, "Recommend 2", ContentType.Movie)
        };
        allContent[0].Genre = "Action";
        allContent[1].Genre = "Action";
        allContent[1].Rating = 4.5;
        allContent[2].Genre = "Comedy";
        allContent[2].Rating = 4.0;

        _viewingHistoryRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(viewingHistory);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(watchedContent);
        _contentRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(allContent);

        // Act - Should not throw StackOverflowException
        Func<Task> act = async () => await _recommendationService.GetRecommendedContentAsync(userId, count);

        // Assert
        await act.Should().NotThrowAsync<StackOverflowException>();

        var result = await _recommendationService.GetRecommendedContentAsync(userId, count);
        result.Should().NotBeNull();
        result.Should().NotContain(c => c.Id == 1); // Should not include watched content
    }

    [Fact]
    public async Task GetRecommendedContentAsync_NoViewingHistory_ShouldReturnPopularContent()
    {
        // Arrange
        const int userId = 999;
        const int count = 5;

        var viewingHistory = new List<ViewingHistory>(); // No viewing history

        var allContent = TestDataBuilder.CreateTestContentList(10);
        allContent[0].Rating = 4.9;
        allContent[1].Rating = 4.7;

        _viewingHistoryRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(viewingHistory);
        _contentRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(allContent);

        // Act
        var result = await _recommendationService.GetRecommendedContentAsync(userId, count);

        // Assert
        result.Should().HaveCount(count);
        (result.First().Rating ?? 0).Should().BeGreaterOrEqualTo(result.Last().Rating ?? 0); // Should be ordered by rating
    }

    [Fact]
    public async Task GetSimilarContentAsync_WhenContentExists_ShouldReturnSimilarContent()
    {
        // Arrange
        const int sourceContentId = 1;
        const int count = 5;

        var sourceContent = TestDataBuilder.CreateTestContent(sourceContentId, "Action Movie", ContentType.Movie);
        sourceContent.Genre = "Action";

        var allContent = new List<Content>
        {
            sourceContent,
            TestDataBuilder.CreateTestContent(2, "Action Movie 2", ContentType.Movie),
            TestDataBuilder.CreateTestContent(3, "Action Movie 3", ContentType.Movie),
            TestDataBuilder.CreateTestContent(4, "Comedy Movie", ContentType.Movie),
            TestDataBuilder.CreateTestContent(5, "Action Series", ContentType.Series)
        };
        allContent[1].Genre = "Action";
        allContent[1].Rating = 4.5;
        allContent[2].Genre = "Action";
        allContent[2].Rating = 4.8;
        allContent[3].Genre = "Comedy";
        allContent[4].Genre = "Action";

        _contentRepositoryMock.Setup(r => r.GetByIdAsync(sourceContentId)).ReturnsAsync(sourceContent);
        _contentRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(allContent);

        // Act
        var result = await _recommendationService.GetSimilarContentAsync(sourceContentId, count);

        // Assert
        var similar = result.ToList();
        similar.Should().NotBeEmpty();
        similar.Should().NotContain(c => c.Id == sourceContentId); // Should not include source content
        similar.Should().Contain(c => c.Genre == "Action" || c.Type == ContentType.Movie);
    }

    [Fact]
    public async Task GetSimilarContentAsync_WhenContentDoesNotExist_ShouldReturnEmpty()
    {
        // Arrange
        const int sourceContentId = 999;
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(sourceContentId)).ReturnsAsync((Content?)null);

        // Act
        var result = await _recommendationService.GetSimilarContentAsync(sourceContentId, 10);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRecommendedByGenreAsync_ShouldReturnContentInGenreNotWatched()
    {
        // Arrange
        const int userId = 1;
        const string genre = "Action";
        const int count = 5;

        var viewingHistory = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, userId, 1)
        };

        var allContent = new List<Content>
        {
            TestDataBuilder.CreateTestContent(1, "Watched Action", ContentType.Movie),
            TestDataBuilder.CreateTestContent(2, "Action Movie 2", ContentType.Movie),
            TestDataBuilder.CreateTestContent(3, "Action Movie 3", ContentType.Movie),
            TestDataBuilder.CreateTestContent(4, "Comedy Movie", ContentType.Movie)
        };
        allContent[0].Genre = "Action"; // Watched
        allContent[1].Genre = "Action";
        allContent[1].Rating = 4.5;
        allContent[2].Genre = "Action";
        allContent[2].Rating = 4.8;
        allContent[3].Genre = "Comedy";

        _viewingHistoryRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(viewingHistory);
        _contentRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(allContent);

        // Act
        var result = await _recommendationService.GetRecommendedByGenreAsync(userId, genre, count);

        // Assert
        var recommendations = result.ToList();
        recommendations.Should().NotBeEmpty();
        recommendations.Should().NotContain(c => c.Id == 1); // Should not include watched
        recommendations.Should().OnlyContain(c => c.Genre == genre);
        (recommendations.First().Rating ?? 0).Should().BeGreaterOrEqualTo(recommendations.Last().Rating ?? 0); // Ordered by rating
    }

    [Fact]
    public async Task GetRecommendedChannelsAsync_WithViewingHistory_ShouldReturnRecommendedChannels()
    {
        // Arrange
        const int userId = 1;
        const int count = 5;

        var viewingHistory = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, userId, null)
        };
        viewingHistory[0].ChannelId = 1;

        var watchedChannel = TestDataBuilder.CreateTestChannel(1, "News Channel 1");
        watchedChannel.Category = "News";

        var allChannels = new List<Channel>
        {
            TestDataBuilder.CreateTestChannel(1, "News 1", true),
            TestDataBuilder.CreateTestChannel(2, "News 2", true),
            TestDataBuilder.CreateTestChannel(3, "News 3", true),
            TestDataBuilder.CreateTestChannel(4, "Sports 1", true),
            TestDataBuilder.CreateTestChannel(5, "Sports 2", true)
        };
        allChannels[0].Category = "News"; // Watched
        allChannels[1].Category = "News";
        allChannels[2].Category = "News";
        allChannels[3].Category = "Sports";
        allChannels[4].Category = "Sports";

        _viewingHistoryRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(viewingHistory);
        _channelRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(watchedChannel);
        _channelRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Channel, bool>>>()))
            .ReturnsAsync(allChannels);

        // Act
        var result = await _recommendationService.GetRecommendedChannelsAsync(userId, count);

        // Assert
        var recommendations = result.ToList();
        recommendations.Should().NotBeEmpty();
        recommendations.Should().NotContain(c => c.Id == 1); // Should not include already watched
        recommendations.Should().Contain(c => c.Category == "News"); // Should prefer News category
    }

    [Fact]
    public async Task GetRecommendedChannelsAsync_StackOverflowFix_ShouldNotThrowStackOverflow()
    {
        // This test specifically verifies the StackOverflow fix at line 132 and 137
        // The fix: materializing queries with .ToList() before using them in Contains()

        // Arrange
        const int userId = 1;
        const int count = 10;

        var viewingHistory = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, userId, null)
        };
        viewingHistory[0].ChannelId = 1;

        var watchedChannel = TestDataBuilder.CreateTestChannel(1, "News Channel");
        watchedChannel.Category = "News";

        // Create just enough channels to trigger the "not enough recommendations" branch
        var allChannels = new List<Channel>
        {
            TestDataBuilder.CreateTestChannel(1, "Watched", true),
            TestDataBuilder.CreateTestChannel(2, "Recommend 1", true),
            TestDataBuilder.CreateTestChannel(3, "Recommend 2", true)
        };
        allChannels[0].Category = "News";
        allChannels[1].Category = "News";
        allChannels[2].Category = "Sports";

        _viewingHistoryRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(viewingHistory);
        _channelRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(watchedChannel);
        _channelRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Channel, bool>>>()))
            .ReturnsAsync(allChannels);

        // Act - Should not throw StackOverflowException
        Func<Task> act = async () => await _recommendationService.GetRecommendedChannelsAsync(userId, count);

        // Assert
        await act.Should().NotThrowAsync<StackOverflowException>();

        var result = await _recommendationService.GetRecommendedChannelsAsync(userId, count);
        result.Should().NotBeNull();
        result.Should().NotContain(c => c.Id == 1); // Should not include watched channel
    }

    [Fact]
    public async Task GetRecommendedChannelsAsync_NoViewingHistory_ShouldReturnActiveChannels()
    {
        // Arrange
        const int userId = 999;
        const int count = 5;

        var viewingHistory = new List<ViewingHistory>(); // No viewing history

        var allChannels = TestDataBuilder.CreateTestChannelList(10);

        _viewingHistoryRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(viewingHistory);
        _channelRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Channel, bool>>>()))
            .ReturnsAsync(allChannels);

        // Act
        var result = await _recommendationService.GetRecommendedChannelsAsync(userId, count);

        // Assert
        result.Should().HaveCount(count);
        result.Should().OnlyContain(c => c.IsActive);
    }
}
