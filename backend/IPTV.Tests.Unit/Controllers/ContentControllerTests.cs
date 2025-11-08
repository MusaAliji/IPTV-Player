using FluentAssertions;
using IPTV.API.Controllers;
using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace IPTV.Tests.Unit.Controllers;

public class ContentControllerTests
{
    private readonly Mock<IContentService> _contentServiceMock;
    private readonly Mock<ILogger<ContentController>> _loggerMock;
    private readonly ContentController _controller;

    public ContentControllerTests()
    {
        _contentServiceMock = new Mock<IContentService>();
        _loggerMock = new Mock<ILogger<ContentController>>();
        _controller = new ContentController(_contentServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetCatalog_ShouldReturnOkWithContent()
    {
        // Arrange
        var testContent = TestDataBuilder.CreateTestContentList(5);
        _contentServiceMock.Setup(s => s.GetAllContentAsync()).ReturnsAsync(testContent);

        // Act
        var result = await _controller.GetCatalog();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContent = okResult.Value.Should().BeAssignableTo<IEnumerable<Content>>().Subject;
        returnedContent.Should().HaveCount(5);
        _contentServiceMock.Verify(s => s.GetAllContentAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCatalog_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        _contentServiceMock.Setup(s => s.GetAllContentAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetCatalog();

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetContent_WhenContentExists_ShouldReturnOkWithContent()
    {
        // Arrange
        const int contentId = 1;
        var testContent = TestDataBuilder.CreateTestContent(contentId);
        _contentServiceMock.Setup(s => s.GetContentByIdAsync(contentId)).ReturnsAsync(testContent);

        // Act
        var result = await _controller.GetContent(contentId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContent = okResult.Value.Should().BeOfType<Content>().Subject;
        returnedContent.Id.Should().Be(contentId);
        _contentServiceMock.Verify(s => s.GetContentByIdAsync(contentId), Times.Once);
    }

    [Fact]
    public async Task GetContent_WhenContentDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int contentId = 999;
        _contentServiceMock.Setup(s => s.GetContentByIdAsync(contentId)).ReturnsAsync((Content?)null);

        // Act
        var result = await _controller.GetContent(contentId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        _contentServiceMock.Verify(s => s.GetContentByIdAsync(contentId), Times.Once);
    }

    [Fact]
    public async Task GetContent_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        const int contentId = 1;
        _contentServiceMock.Setup(s => s.GetContentByIdAsync(contentId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetContent(contentId);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetContentByType_ShouldReturnOkWithContent()
    {
        // Arrange
        const ContentType type = ContentType.Movie;
        var testContent = TestDataBuilder.CreateTestContentList(3);
        _contentServiceMock.Setup(s => s.GetContentByTypeAsync(type)).ReturnsAsync(testContent);

        // Act
        var result = await _controller.GetContentByType(type);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContent = okResult.Value.Should().BeAssignableTo<IEnumerable<Content>>().Subject;
        returnedContent.Should().HaveCount(3);
        _contentServiceMock.Verify(s => s.GetContentByTypeAsync(type), Times.Once);
    }

    [Fact]
    public async Task GetContentByType_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        const ContentType type = ContentType.Movie;
        _contentServiceMock.Setup(s => s.GetContentByTypeAsync(type)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetContentByType(type);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetContentByGenre_ShouldReturnOkWithContent()
    {
        // Arrange
        const string genre = "Action";
        var testContent = TestDataBuilder.CreateTestContentList(3);
        _contentServiceMock.Setup(s => s.GetContentByGenreAsync(genre)).ReturnsAsync(testContent);

        // Act
        var result = await _controller.GetContentByGenre(genre);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContent = okResult.Value.Should().BeAssignableTo<IEnumerable<Content>>().Subject;
        returnedContent.Should().HaveCount(3);
        _contentServiceMock.Verify(s => s.GetContentByGenreAsync(genre), Times.Once);
    }

    [Fact]
    public async Task GetContentByGenre_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        const string genre = "Action";
        _contentServiceMock.Setup(s => s.GetContentByGenreAsync(genre)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetContentByGenre(genre);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task SearchContent_ShouldReturnOkWithContent()
    {
        // Arrange
        const string query = "action";
        var testContent = TestDataBuilder.CreateTestContentList(2);
        _contentServiceMock.Setup(s => s.SearchContentAsync(query)).ReturnsAsync(testContent);

        // Act
        var result = await _controller.SearchContent(query);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContent = okResult.Value.Should().BeAssignableTo<IEnumerable<Content>>().Subject;
        returnedContent.Should().HaveCount(2);
        _contentServiceMock.Verify(s => s.SearchContentAsync(query), Times.Once);
    }

    [Fact]
    public async Task SearchContent_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        const string query = "action";
        _contentServiceMock.Setup(s => s.SearchContentAsync(query)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.SearchContent(query);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetTrending_ShouldReturnOkWithContent()
    {
        // Arrange
        const int count = 10;
        var testContent = TestDataBuilder.CreateTestContentList(count);
        _contentServiceMock.Setup(s => s.GetTrendingContentAsync(count)).ReturnsAsync(testContent);

        // Act
        var result = await _controller.GetTrending(count);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContent = okResult.Value.Should().BeAssignableTo<IEnumerable<Content>>().Subject;
        returnedContent.Should().HaveCount(count);
        _contentServiceMock.Verify(s => s.GetTrendingContentAsync(count), Times.Once);
    }

    [Fact]
    public async Task GetTrending_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        const int count = 10;
        _contentServiceMock.Setup(s => s.GetTrendingContentAsync(count)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetTrending(count);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetRecent_ShouldReturnOkWithContent()
    {
        // Arrange
        const int count = 10;
        var testContent = TestDataBuilder.CreateTestContentList(count);
        _contentServiceMock.Setup(s => s.GetRecentContentAsync(count)).ReturnsAsync(testContent);

        // Act
        var result = await _controller.GetRecent(count);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContent = okResult.Value.Should().BeAssignableTo<IEnumerable<Content>>().Subject;
        returnedContent.Should().HaveCount(count);
        _contentServiceMock.Verify(s => s.GetRecentContentAsync(count), Times.Once);
    }

    [Fact]
    public async Task GetRecent_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        const int count = 10;
        _contentServiceMock.Setup(s => s.GetRecentContentAsync(count)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetRecent(count);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CreateContent_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var newContent = TestDataBuilder.CreateTestContent(0, "New Movie");
        var createdContent = TestDataBuilder.CreateTestContent(1, "New Movie");
        _contentServiceMock.Setup(s => s.CreateContentAsync(newContent)).ReturnsAsync(createdContent);

        // Act
        var result = await _controller.CreateContent(newContent);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(ContentController.GetContent));
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(1);
        var returnedContent = createdResult.Value.Should().BeOfType<Content>().Subject;
        returnedContent.Id.Should().Be(1);
        _contentServiceMock.Verify(s => s.CreateContentAsync(newContent), Times.Once);
    }

    [Fact]
    public async Task CreateContent_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        var newContent = TestDataBuilder.CreateTestContent(0, "New Movie");
        _contentServiceMock.Setup(s => s.CreateContentAsync(newContent)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateContent(newContent);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task UpdateContent_WhenIdMatches_ShouldReturnNoContent()
    {
        // Arrange
        const int contentId = 1;
        var updateContent = TestDataBuilder.CreateTestContent(contentId, "Updated Movie");
        _contentServiceMock.Setup(s => s.UpdateContentAsync(updateContent)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateContent(contentId, updateContent);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _contentServiceMock.Verify(s => s.UpdateContentAsync(updateContent), Times.Once);
    }

    [Fact]
    public async Task UpdateContent_WhenIdDoesNotMatch_ShouldReturnBadRequest()
    {
        // Arrange
        const int contentId = 1;
        var updateContent = TestDataBuilder.CreateTestContent(2, "Updated Movie");

        // Act
        var result = await _controller.UpdateContent(contentId, updateContent);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        _contentServiceMock.Verify(s => s.UpdateContentAsync(It.IsAny<Content>()), Times.Never);
    }

    [Fact]
    public async Task UpdateContent_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        const int contentId = 1;
        var updateContent = TestDataBuilder.CreateTestContent(contentId, "Updated Movie");
        _contentServiceMock.Setup(s => s.UpdateContentAsync(updateContent)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateContent(contentId, updateContent);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task DeleteContent_ShouldReturnNoContent()
    {
        // Arrange
        const int contentId = 1;
        _contentServiceMock.Setup(s => s.DeleteContentAsync(contentId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteContent(contentId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _contentServiceMock.Verify(s => s.DeleteContentAsync(contentId), Times.Once);
    }

    [Fact]
    public async Task DeleteContent_WhenExceptionThrown_ShouldReturn500()
    {
        // Arrange
        const int contentId = 1;
        _contentServiceMock.Setup(s => s.DeleteContentAsync(contentId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteContent(contentId);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }
}
