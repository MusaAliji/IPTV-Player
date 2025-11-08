using IPTV.Core.Entities;
using IPTV.Core.Interfaces;

namespace IPTV.Infrastructure.Services;

public class EPGService : IEPGService
{
    private readonly IUnitOfWork _unitOfWork;

    public EPGService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<EPGProgram>> GetAllProgramsAsync()
    {
        return await _unitOfWork.EPGPrograms.GetAllAsync();
    }

    public async Task<IEnumerable<EPGProgram>> GetProgramsByChannelAsync(int channelId)
    {
        return await _unitOfWork.EPGPrograms.FindAsync(p => p.ChannelId == channelId);
    }

    public async Task<IEnumerable<EPGProgram>> GetCurrentProgramsAsync()
    {
        var now = DateTime.UtcNow;
        return await _unitOfWork.EPGPrograms.FindAsync(p =>
            p.StartTime <= now && p.EndTime >= now
        );
    }

    public async Task<IEnumerable<EPGProgram>> GetProgramsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _unitOfWork.EPGPrograms.FindAsync(p =>
            p.StartTime >= startDate && p.EndTime <= endDate
        );
    }

    public async Task<EPGProgram?> GetCurrentProgramForChannelAsync(int channelId)
    {
        var now = DateTime.UtcNow;
        return await _unitOfWork.EPGPrograms.FirstOrDefaultAsync(p =>
            p.ChannelId == channelId &&
            p.StartTime <= now &&
            p.EndTime >= now
        );
    }

    public async Task<EPGProgram?> GetProgramByIdAsync(int id)
    {
        return await _unitOfWork.EPGPrograms.GetByIdAsync(id);
    }

    public async Task<EPGProgram> CreateProgramAsync(EPGProgram program)
    {
        program.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.EPGPrograms.AddAsync(program);
        await _unitOfWork.SaveChangesAsync();
        return program;
    }

    public async Task UpdateProgramAsync(EPGProgram program)
    {
        _unitOfWork.EPGPrograms.Update(program);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteProgramAsync(int id)
    {
        var program = await _unitOfWork.EPGPrograms.GetByIdAsync(id);
        if (program != null)
        {
            _unitOfWork.EPGPrograms.Remove(program);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Channel>> GetAllChannelsAsync()
    {
        return await _unitOfWork.Channels.GetAllAsync();
    }

    public async Task<Channel?> GetChannelByIdAsync(int id)
    {
        return await _unitOfWork.Channels.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Channel>> GetActiveChannelsAsync()
    {
        return await _unitOfWork.Channels.FindAsync(c => c.IsActive);
    }
}
