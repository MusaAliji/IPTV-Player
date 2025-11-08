using FluentAssertions;
using IPTV.API.Controllers;
using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace IPTV.Tests.Unit.Controllers;

public class AnalyticsControllerTests
{
    private readonly Mock<IAnalyticsService> _analyticsServiceMock;
    private readonly Mock<IRecommendationService> _recommendationServiceMock;
    private readonly AnalyticsController _controller;

    public AnalyticsControllerTests()
    {
        _analyticsServiceMock = new Mock<IAnalyticsService>();
        _recommendationServiceMock = new Mock<IRecommendationService>();
        _controller = new AnalyticsController(_analyticsServiceMock.Object, _recommendationServiceMock.Object);
    }

    private void SetupUserClaims(int userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, "testuser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task StartViewing_ShouldReturnOkWithViewingHistory()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var request = new StartViewingRequest
        {
            ContentId = 1,
            ChannelId = null,
            DeviceInfo = "Test Device"
        };

        var viewingHistory = TestDataBuilder.CreateTestViewingHistory(1, userId, 1);
        _analyticsServiceMock.Setup(s => s.RecordViewingAsync(userId, request.ContentId, request.ChannelId, request.DeviceInfo))
            .ReturnsAsync(viewingHistory);

        // Act
        var result = await _controller.StartViewing(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedHistory = okResult.Value.Should().BeOfType<ViewingHistory>().Subject;
        returnedHistory.Id.Should().Be(1);
        returnedHistory.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task StartViewing_WhenNoUserClaim_ShouldReturnUnauthorized()
    {
        // Arrange - Setup controller context with no claims
        var claims = new List<Claim>(); // Empty claims list
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var request = new StartViewingRequest
        {
            ContentId = 1
        };

        // Act
        var result = await _controller.StartViewing(request);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task UpdateViewingProgress_ShouldReturnNoContent()
    {
        // Arrange
        const int viewingHistoryId = 1;
        var request = new UpdateProgressRequest
        {
            Progress = 3600,
            Completed = false
        };

        _analyticsServiceMock.Setup(s => s.UpdateViewingProgressAsync(viewingHistoryId, request.Progress, request.Completed))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateViewingProgress(viewingHistoryId, request);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _analyticsServiceMock.Verify(s => s.UpdateViewingProgressAsync(viewingHistoryId, request.Progress, request.Completed), Times.Once);
    }

    [Fact]
    public async Task GetViewingHistory_ShouldReturnOkWithHistory()
    {
        // Arrange
        const int userId = 1;
        const int count = 50;
        SetupUserClaims(userId);

        var history = new List<ViewingHistory>
        {
            TestDataBuilder.CreateTestViewingHistory(1, userId, 1),
            TestDataBuilder.CreateTestViewingHistory(2, userId, 2)
        };
        _analyticsServiceMock.Setup(s => s.GetUserViewingHistoryAsync(userId, count)).ReturnsAsync(history);

        // Act
        var result = await _controller.GetViewingHistory(count);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedHistory = okResult.Value.Should().BeAssignableTo<IEnumerable<ViewingHistory>>().Subject;
        returnedHistory.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetViewingHistory_WhenNoUserClaim_ShouldReturnUnauthorized()
    {
        // Arrange - Setup controller context with no claims
        var claims = new List<Claim>(); // Empty claims list
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.GetViewingHistory();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetContinueWatching_ShouldReturnOkWithContent()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var content = TestDataBuilder.CreateTestContentList(3);
        _analyticsServiceMock.Setup(s => s.GetContinueWatchingAsync(userId)).ReturnsAsync(content);

        // Act
        var result = await _controller.GetContinueWatching();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContent = okResult.Value.Should().BeAssignableTo<IEnumerable<Content>>().Subject;
        returnedContent.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetViewingStatsByGenre_ShouldReturnOkWithStats()
    {
        // Arrange
        const int userId = 1;
        SetupUserClaims(userId);

        var stats = new Dictionary<string, int>
        {
            { "Action", 10 },
            { "Comedy", 5 },
            { "Drama", 3 }
        };
        _analyticsServiceMock.Setup(s => s.GetViewingStatsByGenreAsync(userId)).ReturnsAsync(stats);

        // Act
        var result = await _controller.GetViewingStatsByGenre();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedStats = okResult.Value.Should().BeAssignableTo<Dictionary<string, int>>().Subject;
        returnedStats.Should().HaveCount(3);
        returnedStats["Action"].Should().Be(10);
    }

    [Fact]
    public async Task GetTotalViewingTime_ShouldReturnOkWithTotalTime()
    {
        // Arrange
        const int userId = 1;
        const int totalTime = 7200;
        SetupUserClaims(userId);

        _analyticsServiceMock.Setup(s => s.GetTotalViewingTimeAsync(userId)).ReturnsAsync(totalTime);

        // Act
        var result = await _controller.GetTotalViewingTime();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        value.Should().NotBeNull();

        var valueType = value!.GetType();
        var totalSeconds = valueType.GetProperty("totalSeconds")?.GetValue(value);
        totalSeconds.Should().Be(totalTime);
    }

    [Fact]
    public async Task GetMostWatchedContent_ShouldReturnOkWithStats()
    {
        // Arrange
        const int count = 10;
        var stats = new Dictionary<string, int>
        {
            { "Content 1", 100 },
            { "Content 2", 75 },
            { "Content 3", 50 }
        };
        _analyticsServiceMock.Setup(s => s.GetMostWatchedContentAsync(count)).ReturnsAsync(stats);

        // Act
        var result = await _controller.GetMostWatchedContent(count);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedStats = okResult.Value.Should().BeAssignableTo<Dictionary<string, int>>().Subject;
        returnedStats.Should().HaveCount(3);
        returnedStats["Content 1"].Should().Be(100);
    }

    [Fact]
    public async Task GetMostWatchedChannels_ShouldReturnOkWithStats()
    {
        // Arrange
        const int count = 10;
        var stats = new Dictionary<string, int>
        {
            { "Channel 1", 200 },
            { "Channel 2", 150 },
            { "Channel 3", 100 }
        };
        _analyticsServiceMock.Setup(s => s.GetMostWatchedChannelsAsync(count)).ReturnsAsync(stats);

        // Act
        var result = await _controller.GetMostWatchedChannels(count);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedStats = okResult.Value.Should().BeAssignableTo<Dictionary<string, int>>().Subject;
        returnedStats.Should().HaveCount(3);
        returnedStats["Channel 1"].Should().Be(200);
    }

    [Fact]
    public async Task GetRecommendations_ShouldReturnOkWithContent()
    {
        // Arrange
        const int userId = 1;
        const int count = 10;
        SetupUserClaims(userId);

        var recommendations = TestDataBuilder.CreateTestContentList(count);
        _recommendationServiceMock.Setup(s => s.GetRecommendedContentAsync(userId, count)).ReturnsAsync(recommendations);

        // Act
        var result = await _controller.GetRecommendations(count);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContent = okResult.Value.Should().BeAssignableTo<IEnumerable<Content>>().Subject;
        returnedContent.Should().HaveCount(count);
    }

    [Fact]
    public async Task GetRecommendations_WhenNoUserClaim_ShouldReturnUnauthorized()
    {
        // Arrange - Setup controller context with no claims
        var claims = new List<Claim>(); // Empty claims list
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.GetRecommendations();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task GetSimilarContent_ShouldReturnOkWithContent()
    {
        // Arrange
        const int contentId = 1;
        const int count = 10;

        var similarContent = TestDataBuilder.CreateTestContentList(count);
        _recommendationServiceMock.Setup(s => s.GetSimilarContentAsync(contentId, count)).ReturnsAsync(similarContent);

        // Act
        var result = await _controller.GetSimilarContent(contentId, count);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContent = okResult.Value.Should().BeAssignableTo<IEnumerable<Content>>().Subject;
        returnedContent.Should().HaveCount(count);
    }

    [Fact]
    public async Task GetRecommendedChannels_ShouldReturnOkWithChannels()
    {
        // Arrange
        const int userId = 1;
        const int count = 10;
        SetupUserClaims(userId);

        var channels = TestDataBuilder.CreateTestChannelList(count);
        _recommendationServiceMock.Setup(s => s.GetRecommendedChannelsAsync(userId, count)).ReturnsAsync(channels);

        // Act
        var result = await _controller.GetRecommendedChannels(count);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedChannels = okResult.Value.Should().BeAssignableTo<IEnumerable<Channel>>().Subject;
        returnedChannels.Should().HaveCount(count);
    }

    [Fact]
    public async Task GetRecommendedChannels_WhenNoUserClaim_ShouldReturnUnauthorized()
    {
        // Arrange - Setup controller context with no claims
        var claims = new List<Claim>(); // Empty claims list
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.GetRecommendedChannels();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }
}
