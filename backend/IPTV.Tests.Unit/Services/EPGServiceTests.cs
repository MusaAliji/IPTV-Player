using FluentAssertions;
using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Infrastructure.Services;
using IPTV.Tests.Unit.Helpers;
using Moq;

namespace IPTV.Tests.Unit.Services;

public class EPGServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<EPGProgram>> _epgProgramRepositoryMock;
    private readonly Mock<IRepository<Channel>> _channelRepositoryMock;
    private readonly EPGService _epgService;

    public EPGServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _epgProgramRepositoryMock = new Mock<IRepository<EPGProgram>>();
        _channelRepositoryMock = new Mock<IRepository<Channel>>();

        _unitOfWorkMock.Setup(u => u.EPGPrograms).Returns(_epgProgramRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Channels).Returns(_channelRepositoryMock.Object);

        _epgService = new EPGService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetAllProgramsAsync_ShouldReturnAllPrograms()
    {
        // Arrange
        var testPrograms = TestDataBuilder.CreateTestEPGProgramList(5);
        _epgProgramRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(testPrograms);

        // Act
        var result = await _epgService.GetAllProgramsAsync();

        // Assert
        result.Should().HaveCount(5);
        result.Should().BeEquivalentTo(testPrograms);
        _epgProgramRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProgramsByChannelAsync_ShouldReturnProgramsForSpecificChannel()
    {
        // Arrange
        const int channelId = 1;
        var testPrograms = TestDataBuilder.CreateTestEPGProgramList(3, channelId);
        _epgProgramRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EPGProgram, bool>>>()))
            .ReturnsAsync(testPrograms);

        // Act
        var result = await _epgService.GetProgramsByChannelAsync(channelId);

        // Assert
        result.Should().HaveCount(3);
        result.Should().OnlyContain(p => p.ChannelId == channelId);
        _epgProgramRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EPGProgram, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetCurrentProgramsAsync_ShouldReturnProgramsAiringNow()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var currentPrograms = new List<EPGProgram>
        {
            TestDataBuilder.CreateTestEPGProgram(1, 1, now.AddMinutes(-30), now.AddMinutes(30)),
            TestDataBuilder.CreateTestEPGProgram(2, 2, now.AddMinutes(-15), now.AddMinutes(45))
        };
        _epgProgramRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EPGProgram, bool>>>()))
            .ReturnsAsync(currentPrograms);

        // Act
        var result = await _epgService.GetCurrentProgramsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(currentPrograms);
        _epgProgramRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EPGProgram, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetProgramsByDateRangeAsync_ShouldReturnProgramsInRange()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(7);
        var programsInRange = new List<EPGProgram>
        {
            TestDataBuilder.CreateTestEPGProgram(1, 1, startDate.AddDays(1), startDate.AddDays(1).AddHours(2)),
            TestDataBuilder.CreateTestEPGProgram(2, 1, startDate.AddDays(2), startDate.AddDays(2).AddHours(2))
        };
        _epgProgramRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EPGProgram, bool>>>()))
            .ReturnsAsync(programsInRange);

        // Act
        var result = await _epgService.GetProgramsByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(programsInRange);
        _epgProgramRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EPGProgram, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetCurrentProgramForChannelAsync_ShouldReturnCurrentProgram()
    {
        // Arrange
        const int channelId = 1;
        var now = DateTime.UtcNow;
        var currentProgram = TestDataBuilder.CreateTestEPGProgram(1, channelId, now.AddMinutes(-30), now.AddMinutes(30));
        _epgProgramRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EPGProgram, bool>>>()))
            .ReturnsAsync(currentProgram);

        // Act
        var result = await _epgService.GetCurrentProgramForChannelAsync(channelId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(currentProgram);
        result!.ChannelId.Should().Be(channelId);
        _epgProgramRepositoryMock.Verify(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EPGProgram, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetCurrentProgramForChannelAsync_WhenNoCurrentProgram_ShouldReturnNull()
    {
        // Arrange
        const int channelId = 1;
        _epgProgramRepositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EPGProgram, bool>>>()))
            .ReturnsAsync((EPGProgram?)null);

        // Act
        var result = await _epgService.GetCurrentProgramForChannelAsync(channelId);

        // Assert
        result.Should().BeNull();
        _epgProgramRepositoryMock.Verify(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<EPGProgram, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task GetProgramByIdAsync_ShouldReturnProgram()
    {
        // Arrange
        const int programId = 1;
        var testProgram = TestDataBuilder.CreateTestEPGProgram(programId);
        _epgProgramRepositoryMock.Setup(r => r.GetByIdAsync(programId)).ReturnsAsync(testProgram);

        // Act
        var result = await _epgService.GetProgramByIdAsync(programId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(testProgram);
        _epgProgramRepositoryMock.Verify(r => r.GetByIdAsync(programId), Times.Once);
    }

    [Fact]
    public async Task CreateProgramAsync_ShouldCreateProgramWithCreatedAt()
    {
        // Arrange
        var newProgram = TestDataBuilder.CreateTestEPGProgram(0);
        newProgram.CreatedAt = default;
        _epgProgramRepositoryMock.Setup(r => r.AddAsync(It.IsAny<EPGProgram>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _epgService.CreateProgramAsync(newProgram);

        // Assert
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        _epgProgramRepositoryMock.Verify(r => r.AddAsync(newProgram), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProgramAsync_ShouldUpdateProgram()
    {
        // Arrange
        var existingProgram = TestDataBuilder.CreateTestEPGProgram(1);
        existingProgram.Title = "Updated Title";
        _epgProgramRepositoryMock.Setup(r => r.Update(It.IsAny<EPGProgram>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _epgService.UpdateProgramAsync(existingProgram);

        // Assert
        _epgProgramRepositoryMock.Verify(r => r.Update(existingProgram), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteProgramAsync_WhenProgramExists_ShouldDeleteProgram()
    {
        // Arrange
        const int programId = 1;
        var existingProgram = TestDataBuilder.CreateTestEPGProgram(programId);
        _epgProgramRepositoryMock.Setup(r => r.GetByIdAsync(programId)).ReturnsAsync(existingProgram);
        _epgProgramRepositoryMock.Setup(r => r.Remove(It.IsAny<EPGProgram>()));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _epgService.DeleteProgramAsync(programId);

        // Assert
        _epgProgramRepositoryMock.Verify(r => r.GetByIdAsync(programId), Times.Once);
        _epgProgramRepositoryMock.Verify(r => r.Remove(existingProgram), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteProgramAsync_WhenProgramDoesNotExist_ShouldNotDelete()
    {
        // Arrange
        const int programId = 999;
        _epgProgramRepositoryMock.Setup(r => r.GetByIdAsync(programId)).ReturnsAsync((EPGProgram?)null);

        // Act
        await _epgService.DeleteProgramAsync(programId);

        // Assert
        _epgProgramRepositoryMock.Verify(r => r.GetByIdAsync(programId), Times.Once);
        _epgProgramRepositoryMock.Verify(r => r.Remove(It.IsAny<EPGProgram>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllChannelsAsync_ShouldReturnAllChannels()
    {
        // Arrange
        var testChannels = TestDataBuilder.CreateTestChannelList(5);
        _channelRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(testChannels);

        // Act
        var result = await _epgService.GetAllChannelsAsync();

        // Assert
        result.Should().HaveCount(5);
        result.Should().BeEquivalentTo(testChannels);
        _channelRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetChannelByIdAsync_ShouldReturnChannel()
    {
        // Arrange
        const int channelId = 1;
        var testChannel = TestDataBuilder.CreateTestChannel(channelId);
        _channelRepositoryMock.Setup(r => r.GetByIdAsync(channelId)).ReturnsAsync(testChannel);

        // Act
        var result = await _epgService.GetChannelByIdAsync(channelId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(testChannel);
        _channelRepositoryMock.Verify(r => r.GetByIdAsync(channelId), Times.Once);
    }

    [Fact]
    public async Task GetActiveChannelsAsync_ShouldReturnOnlyActiveChannels()
    {
        // Arrange
        var activeChannels = new List<Channel>
        {
            TestDataBuilder.CreateTestChannel(1, true),
            TestDataBuilder.CreateTestChannel(2, true)
        };
        _channelRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Channel, bool>>>()))
            .ReturnsAsync(activeChannels);

        // Act
        var result = await _epgService.GetActiveChannelsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.IsActive);
        _channelRepositoryMock.Verify(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Channel, bool>>>()), Times.Once);
    }
}
