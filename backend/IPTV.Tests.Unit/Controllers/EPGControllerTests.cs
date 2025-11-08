using FluentAssertions;
using IPTV.API.Controllers;
using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace IPTV.Tests.Unit.Controllers;

public class EPGControllerTests
{
    private readonly Mock<IEPGService> _epgServiceMock;
    private readonly EPGController _controller;

    public EPGControllerTests()
    {
        _epgServiceMock = new Mock<IEPGService>();
        _controller = new EPGController(_epgServiceMock.Object);
    }

    [Fact]
    public async Task GetAllPrograms_ShouldReturnOkWithPrograms()
    {
        // Arrange
        var programs = TestDataBuilder.CreateTestEPGProgramList(5);
        _epgServiceMock.Setup(s => s.GetAllProgramsAsync()).ReturnsAsync(programs);

        // Act
        var result = await _controller.GetAllPrograms();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPrograms = okResult.Value.Should().BeAssignableTo<IEnumerable<EPGProgram>>().Subject;
        returnedPrograms.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetProgramById_WhenProgramExists_ShouldReturnOkWithProgram()
    {
        // Arrange
        const int programId = 1;
        var program = TestDataBuilder.CreateTestEPGProgram(programId);
        _epgServiceMock.Setup(s => s.GetProgramByIdAsync(programId)).ReturnsAsync(program);

        // Act
        var result = await _controller.GetProgramById(programId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProgram = okResult.Value.Should().BeOfType<EPGProgram>().Subject;
        returnedProgram.Id.Should().Be(programId);
    }

    [Fact]
    public async Task GetProgramById_WhenProgramDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int programId = 999;
        _epgServiceMock.Setup(s => s.GetProgramByIdAsync(programId)).ReturnsAsync((EPGProgram?)null);

        // Act
        var result = await _controller.GetProgramById(programId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetProgramsByChannel_ShouldReturnOkWithPrograms()
    {
        // Arrange
        const int channelId = 1;
        var programs = TestDataBuilder.CreateTestEPGProgramList(3, channelId);
        _epgServiceMock.Setup(s => s.GetProgramsByChannelAsync(channelId)).ReturnsAsync(programs);

        // Act
        var result = await _controller.GetProgramsByChannel(channelId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPrograms = okResult.Value.Should().BeAssignableTo<IEnumerable<EPGProgram>>().Subject;
        returnedPrograms.Should().HaveCount(3);
        returnedPrograms.Should().OnlyContain(p => p.ChannelId == channelId);
    }

    [Fact]
    public async Task GetCurrentPrograms_ShouldReturnOkWithPrograms()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var programs = new List<EPGProgram>
        {
            TestDataBuilder.CreateTestEPGProgram(1, 1, now.AddMinutes(-30), now.AddMinutes(30)),
            TestDataBuilder.CreateTestEPGProgram(2, 2, now.AddMinutes(-15), now.AddMinutes(45))
        };
        _epgServiceMock.Setup(s => s.GetCurrentProgramsAsync()).ReturnsAsync(programs);

        // Act
        var result = await _controller.GetCurrentPrograms();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPrograms = okResult.Value.Should().BeAssignableTo<IEnumerable<EPGProgram>>().Subject;
        returnedPrograms.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCurrentProgramForChannel_WhenProgramExists_ShouldReturnOkWithProgram()
    {
        // Arrange
        const int channelId = 1;
        var now = DateTime.UtcNow;
        var program = TestDataBuilder.CreateTestEPGProgram(1, channelId, now.AddMinutes(-30), now.AddMinutes(30));
        _epgServiceMock.Setup(s => s.GetCurrentProgramForChannelAsync(channelId)).ReturnsAsync(program);

        // Act
        var result = await _controller.GetCurrentProgramForChannel(channelId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProgram = okResult.Value.Should().BeOfType<EPGProgram>().Subject;
        returnedProgram.ChannelId.Should().Be(channelId);
    }

    [Fact]
    public async Task GetCurrentProgramForChannel_WhenProgramDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int channelId = 999;
        _epgServiceMock.Setup(s => s.GetCurrentProgramForChannelAsync(channelId)).ReturnsAsync((EPGProgram?)null);

        // Act
        var result = await _controller.GetCurrentProgramForChannel(channelId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetProgramsByDateRange_ShouldReturnOkWithPrograms()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(7);
        var programs = TestDataBuilder.CreateTestEPGProgramList(5);
        _epgServiceMock.Setup(s => s.GetProgramsByDateRangeAsync(startDate, endDate)).ReturnsAsync(programs);

        // Act
        var result = await _controller.GetProgramsByDateRange(startDate, endDate);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedPrograms = okResult.Value.Should().BeAssignableTo<IEnumerable<EPGProgram>>().Subject;
        returnedPrograms.Should().HaveCount(5);
    }

    [Fact]
    public async Task CreateProgram_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var newProgram = TestDataBuilder.CreateTestEPGProgram(0);
        var createdProgram = TestDataBuilder.CreateTestEPGProgram(1);
        _epgServiceMock.Setup(s => s.CreateProgramAsync(newProgram)).ReturnsAsync(createdProgram);

        // Act
        var result = await _controller.CreateProgram(newProgram);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(EPGController.GetProgramById));
        createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(1);
        var returnedProgram = createdResult.Value.Should().BeOfType<EPGProgram>().Subject;
        returnedProgram.Id.Should().Be(1);
    }

    [Fact]
    public async Task UpdateProgram_WhenIdMatches_ShouldReturnNoContent()
    {
        // Arrange
        const int programId = 1;
        var updateProgram = TestDataBuilder.CreateTestEPGProgram(programId);
        updateProgram.Title = "Updated Title";
        _epgServiceMock.Setup(s => s.UpdateProgramAsync(updateProgram)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateProgram(programId, updateProgram);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _epgServiceMock.Verify(s => s.UpdateProgramAsync(updateProgram), Times.Once);
    }

    [Fact]
    public async Task UpdateProgram_WhenIdDoesNotMatch_ShouldReturnBadRequest()
    {
        // Arrange
        const int programId = 1;
        var updateProgram = TestDataBuilder.CreateTestEPGProgram(2);

        // Act
        var result = await _controller.UpdateProgram(programId, updateProgram);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        _epgServiceMock.Verify(s => s.UpdateProgramAsync(It.IsAny<EPGProgram>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProgram_ShouldReturnNoContent()
    {
        // Arrange
        const int programId = 1;
        _epgServiceMock.Setup(s => s.DeleteProgramAsync(programId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteProgram(programId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _epgServiceMock.Verify(s => s.DeleteProgramAsync(programId), Times.Once);
    }

    [Fact]
    public async Task GetAllChannels_ShouldReturnOkWithChannels()
    {
        // Arrange
        var channels = TestDataBuilder.CreateTestChannelList(5);
        _epgServiceMock.Setup(s => s.GetAllChannelsAsync()).ReturnsAsync(channels);

        // Act
        var result = await _controller.GetAllChannels();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedChannels = okResult.Value.Should().BeAssignableTo<IEnumerable<Channel>>().Subject;
        returnedChannels.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetChannelById_WhenChannelExists_ShouldReturnOkWithChannel()
    {
        // Arrange
        const int channelId = 1;
        var channel = TestDataBuilder.CreateTestChannel(channelId);
        _epgServiceMock.Setup(s => s.GetChannelByIdAsync(channelId)).ReturnsAsync(channel);

        // Act
        var result = await _controller.GetChannelById(channelId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedChannel = okResult.Value.Should().BeOfType<Channel>().Subject;
        returnedChannel.Id.Should().Be(channelId);
    }

    [Fact]
    public async Task GetChannelById_WhenChannelDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        const int channelId = 999;
        _epgServiceMock.Setup(s => s.GetChannelByIdAsync(channelId)).ReturnsAsync((Channel?)null);

        // Act
        var result = await _controller.GetChannelById(channelId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetActiveChannels_ShouldReturnOkWithActiveChannels()
    {
        // Arrange
        var activeChannels = new List<Channel>
        {
            TestDataBuilder.CreateTestChannel(1, true),
            TestDataBuilder.CreateTestChannel(2, true),
            TestDataBuilder.CreateTestChannel(3, true)
        };
        _epgServiceMock.Setup(s => s.GetActiveChannelsAsync()).ReturnsAsync(activeChannels);

        // Act
        var result = await _controller.GetActiveChannels();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedChannels = okResult.Value.Should().BeAssignableTo<IEnumerable<Channel>>().Subject;
        returnedChannels.Should().HaveCount(3);
        returnedChannels.Should().OnlyContain(c => c.IsActive);
    }
}
