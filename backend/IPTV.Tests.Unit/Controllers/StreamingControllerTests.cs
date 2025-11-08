using FluentAssertions;
using IPTV.API.Controllers;
using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace IPTV.Tests.Unit.Controllers;

public class StreamingControllerTests
{
    private readonly Mock<IContentService> _contentServiceMock;
    private readonly Mock<IEPGService> _epgServiceMock;
    private readonly StreamingController _controller;

    public StreamingControllerTests()
    {
        _contentServiceMock = new Mock<IContentService>();
        _epgServiceMock = new Mock<IEPGService>();
        _controller = new StreamingController(_contentServiceMock.Object, _epgServiceMock.Object);
    }

    [Fact]
    public async Task GetContentStreamUrl_WhenContentExists_ShouldReturnOkWithStreamUrl()
    {
        // Arrange
        const int contentId = 1;
        var content = TestDataBuilder.CreateTestContent(contentId, "Test Movie", ContentType.Movie);
        content.StreamUrl = "https://test.com/stream.m3u8";
        _contentServiceMock.Setup(s => s.GetContentByIdAsync(contentId)).ReturnsAsync(content);

        // Act
        var result = await _controller.GetContentStreamUrl(contentId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<StreamUrlResponse>().Subject;
        response.ContentId.Should().Be(contentId);
        response.StreamUrl.Should().Be("https://test.com/stream.m3u8");
        response.ContentType.Should().Be("Movie");
        response.Title.Should().Be("Test Movie");
    }

    [Fact]
    public async Task GetContentStreamUrl_WhenContentDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int contentId = 999;
        _contentServiceMock.Setup(s => s.GetContentByIdAsync(contentId)).ReturnsAsync((Content?)null);

        // Act
        var result = await _controller.GetContentStreamUrl(contentId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetChannelStreamUrl_WhenChannelExists_ShouldReturnOkWithStreamUrl()
    {
        // Arrange
        const int channelId = 1;
        var channel = TestDataBuilder.CreateTestChannel(channelId, "Test Channel");
        channel.StreamUrl = "https://test.com/channel.m3u8";
        _epgServiceMock.Setup(s => s.GetChannelByIdAsync(channelId)).ReturnsAsync(channel);

        // Act
        var result = await _controller.GetChannelStreamUrl(channelId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<StreamUrlResponse>().Subject;
        response.ChannelId.Should().Be(channelId);
        response.StreamUrl.Should().Be("https://test.com/channel.m3u8");
        response.ContentType.Should().Be("LiveTV");
        response.Title.Should().Be("Test Channel");
    }

    [Fact]
    public async Task GetChannelStreamUrl_WhenChannelDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int channelId = 999;
        _epgServiceMock.Setup(s => s.GetChannelByIdAsync(channelId)).ReturnsAsync((Channel?)null);

        // Act
        var result = await _controller.GetChannelStreamUrl(channelId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetContentManifest_WhenContentExists_ShouldReturnOkWithManifest()
    {
        // Arrange
        const int contentId = 1;
        var content = TestDataBuilder.CreateTestContent(contentId);
        content.StreamUrl = "https://test.com/stream";
        _contentServiceMock.Setup(s => s.GetContentByIdAsync(contentId)).ReturnsAsync(content);

        // Act
        var result = await _controller.GetContentManifest(contentId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        value.Should().NotBeNull();

        var valueType = value!.GetType();
        var manifestUrl = valueType.GetProperty("manifestUrl")?.GetValue(value) as string;
        var type = valueType.GetProperty("type")?.GetValue(value) as string;

        manifestUrl.Should().Be("https://test.com/stream/manifest.m3u8");
        type.Should().Be("application/vnd.apple.mpegurl");
    }

    [Fact]
    public async Task GetContentManifest_WhenContentDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int contentId = 999;
        _contentServiceMock.Setup(s => s.GetContentByIdAsync(contentId)).ReturnsAsync((Content?)null);

        // Act
        var result = await _controller.GetContentManifest(contentId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetChannelManifest_WhenChannelExists_ShouldReturnOkWithManifest()
    {
        // Arrange
        const int channelId = 1;
        var channel = TestDataBuilder.CreateTestChannel(channelId);
        channel.StreamUrl = "https://test.com/channel";
        _epgServiceMock.Setup(s => s.GetChannelByIdAsync(channelId)).ReturnsAsync(channel);

        // Act
        var result = await _controller.GetChannelManifest(channelId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value;
        value.Should().NotBeNull();

        var valueType = value!.GetType();
        var manifestUrl = valueType.GetProperty("manifestUrl")?.GetValue(value) as string;
        var type = valueType.GetProperty("type")?.GetValue(value) as string;

        manifestUrl.Should().Be("https://test.com/channel/manifest.m3u8");
        type.Should().Be("application/vnd.apple.mpegurl");
    }

    [Fact]
    public async Task GetChannelManifest_WhenChannelDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int channelId = 999;
        _epgServiceMock.Setup(s => s.GetChannelByIdAsync(channelId)).ReturnsAsync((Channel?)null);

        // Act
        var result = await _controller.GetChannelManifest(channelId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}
