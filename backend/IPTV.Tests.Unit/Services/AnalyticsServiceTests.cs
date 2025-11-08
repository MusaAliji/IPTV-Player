using FluentAssertions;
using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Infrastructure.Services;
using IPTV.Tests.Unit.Helpers;
using Moq;
using Xunit;

namespace IPTV.Tests.Unit.Services;

public class AnalyticsServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<ViewingHistory>> _viewingRepositoryMock;
    private readonly Mock<IRepository<Content>> _contentRepositoryMock;
    private readonly Mock<IRepository<Channel>> _channelRepositoryMock;
    private readonly AnalyticsService _analyticsService;

    public AnalyticsServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _viewingRepositoryMock = new Mock<IRepository<ViewingHistory>>();
        _contentRepositoryMock = new Mock<IRepository<Content>>();
        _channelRepositoryMock = new Mock<IRepository<Channel>>();

        _unitOfWorkMock.Setup(u => u.ViewingHistories).Returns(_viewingRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Contents).Returns(_contentRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Channels).Returns(_channelRepositoryMock.Object);

        _analyticsService = new AnalyticsService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RecordViewingAsync_ShouldCreateViewingHistory()
    {
        // Arrange
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _analyticsService.RecordViewingAsync(1, 1, null, "Web Browser");

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(1);
        result.ContentId.Should().Be(1);
        result.DeviceInfo.Should().Be("Web Browser");
        result.Completed.Should().BeFalse();
        _viewingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ViewingHistory>()), Times.Once);
    }

    [Fact]
    public async Task UpdateViewingProgressAsync_ShouldUpdateProgress()
    {
        // Arrange
        var viewing = TestDataBuilder.CreateTestViewingHistory();
        viewing.EndTime = null; // Start with null EndTime for incomplete viewing
        _viewingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(viewing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _analyticsService.UpdateViewingProgressAsync(1, 1800, false);

        // Assert
        viewing.Progress.Should().Be(1800);
        viewing.Completed.Should().BeFalse();
        viewing.EndTime.Should().BeNull();
    }

    [Fact]
    public async Task UpdateViewingProgressAsync_ShouldSetDuration_WhenCompleted()
    {
        // Arrange
        var viewing = TestDataBuilder.CreateTestViewingHistory();
        viewing.StartTime = DateTime.UtcNow.AddHours(-2);
        _viewingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(viewing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _analyticsService.UpdateViewingProgressAsync(1, 7200, true);

        // Assert
        viewing.Completed.Should().BeTrue();
        viewing.EndTime.Should().NotBeNull();
        viewing.Duration.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetUserViewingHistoryAsync_ShouldReturnHistory()
    {
        // Arrange
        var history = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, 1, 1),
            TestDataBuilder.CreateTestViewingHistory(2, 1, 2),
            TestDataBuilder.CreateTestViewingHistory(3, 1, 3)
        };
        _viewingRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(history);

        // Act
        var result = await _analyticsService.GetUserViewingHistoryAsync(1, 10);

        // Assert
        result.Should().HaveCountLessOrEqualTo(10);
    }

    [Fact]
    public async Task GetViewingStatsByGenreAsync_ShouldReturnStatsDictionary()
    {
        // Arrange
        var viewingHistory = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, 1, 1),
            TestDataBuilder.CreateTestViewingHistory(2, 1, 2),
            TestDataBuilder.CreateTestViewingHistory(4, 1, 3),
            TestDataBuilder.CreateTestViewingHistory(3, 1, 1)
        };
        var content1 = TestDataBuilder.CreateTestContent(1);
        content1.Genre = "Action";
        var content2 = TestDataBuilder.CreateTestContent(2);
        var content3 = TestDataBuilder.CreateTestContent(3);
        content3.Genre = "Action";
        content2.Genre = "Comedy";

        _viewingRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(viewingHistory);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(content1);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(content2);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(content3);

        // Act
        var result = await _analyticsService.GetViewingStatsByGenreAsync(1);

        // Assert
        result.Should().ContainKey("Action");
        result.Should().ContainKey("Comedy");
        result["Action"].Should().Be(2);
        result["Comedy"].Should().Be(1);
    }

    [Fact]
    public async Task GetMostWatchedContentAsync_ShouldReturnTopContent()
    {
        // Arrange
        var allHistory = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, 1, 1),
            TestDataBuilder.CreateTestViewingHistory(2, 2, 1),
            TestDataBuilder.CreateTestViewingHistory(3, 1, 2),
            TestDataBuilder.CreateTestViewingHistory(4, 2, 1)
        };
        var content1 = TestDataBuilder.CreateTestContent(1, "Popular Content");
        var content2 = TestDataBuilder.CreateTestContent(2, "Less Popular");

        _viewingRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(allHistory);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(content1);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(content2);

        // Act
        var result = await _analyticsService.GetMostWatchedContentAsync(10);

        // Assert
        result.Should().ContainKey("Popular Content");
        result["Popular Content"].Should().Be(3);
    }

    [Fact]
    public async Task GetMostWatchedChannelsAsync_ShouldReturnTopChannels()
    {
        // Arrange
        var allHistory = new List<ViewingHistory>
        {
            new ViewingHistory { Id = 1, UserId = 1, ChannelId = 1, StartTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new ViewingHistory { Id = 2, UserId = 2, ChannelId = 1, StartTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new ViewingHistory { Id = 3, UserId = 1, ChannelId = 2, StartTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
        };
        var channel1 = TestDataBuilder.CreateTestChannel(1, "Popular Channel");
        var channel2 = TestDataBuilder.CreateTestChannel(2, "Less Popular");

        _viewingRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(allHistory);
        _channelRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(channel1);
        _channelRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(channel2);

        // Act
        var result = await _analyticsService.GetMostWatchedChannelsAsync(10);

        // Assert
        result.Should().ContainKey("Popular Channel");
        result["Popular Channel"].Should().Be(2);
    }

    [Fact]
    public async Task GetTotalViewingTimeAsync_ShouldReturnTotalSeconds()
    {
        // Arrange
        var history = new List<ViewingHistory>
        {
            new ViewingHistory { Id = 1, UserId = 1, Duration = 3600, StartTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new ViewingHistory { Id = 2, UserId = 1, Duration = 7200, StartTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new ViewingHistory { Id = 3, UserId = 1, Duration = null, StartTime = DateTime.UtcNow, CreatedAt = DateTime.UtcNow }
        };
        _viewingRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(history.Where(h => h.Duration.HasValue));

        // Act
        var result = await _analyticsService.GetTotalViewingTimeAsync(1);

        // Assert
        result.Should().Be(10800); // 3600 + 7200
    }

    [Fact]
    public async Task GetContinueWatchingAsync_ShouldReturnIncompleteContent()
    {
        // Arrange
        var incompleteHistory = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, 1, 1),
            TestDataBuilder.CreateTestViewingHistory(2, 1, 2)
        };
        incompleteHistory.ForEach(h => h.Progress = 1800);

        var content1 = TestDataBuilder.CreateTestContent(1);
        var content2 = TestDataBuilder.CreateTestContent(2);

        _viewingRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(incompleteHistory);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(content1);
        _contentRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(content2);

        // Act
        var result = await _analyticsService.GetContinueWatchingAsync(1);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCountLessOrEqualTo(10);
    }

    [Fact]
    public async Task GetLastViewingForContentAsync_ShouldReturnMostRecentViewing()
    {
        // Arrange
        var history = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, 1, 1),
            TestDataBuilder.CreateTestViewingHistory(2, 1, 1)
        };
        history[0].StartTime = DateTime.UtcNow.AddHours(-2);
        history[1].StartTime = DateTime.UtcNow.AddHours(-1);

        _viewingRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ViewingHistory, bool>>>()))
            .ReturnsAsync(history);

        // Act
        var result = await _analyticsService.GetLastViewingForContentAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(2); // Most recent
    }
}
