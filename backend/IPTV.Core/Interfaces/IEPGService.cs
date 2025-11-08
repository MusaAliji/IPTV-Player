using IPTV.Core.Entities;

namespace IPTV.Core.Interfaces;

public interface IEPGService
{
    Task<IEnumerable<EPGProgram>> GetAllProgramsAsync();
    Task<IEnumerable<EPGProgram>> GetProgramsByChannelAsync(int channelId);
    Task<IEnumerable<EPGProgram>> GetCurrentProgramsAsync();
    Task<IEnumerable<EPGProgram>> GetProgramsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<EPGProgram?> GetCurrentProgramForChannelAsync(int channelId);
    Task<EPGProgram?> GetProgramByIdAsync(int id);
    Task<EPGProgram> CreateProgramAsync(EPGProgram program);
    Task UpdateProgramAsync(EPGProgram program);
    Task DeleteProgramAsync(int id);
    Task<IEnumerable<Channel>> GetAllChannelsAsync();
    Task<Channel?> GetChannelByIdAsync(int id);
    Task<IEnumerable<Channel>> GetActiveChannelsAsync();
}
