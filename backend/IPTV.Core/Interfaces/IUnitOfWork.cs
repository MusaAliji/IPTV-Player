using IPTV.Core.Entities;

namespace IPTV.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Content> Contents { get; }
    IRepository<Channel> Channels { get; }
    IRepository<EPGProgram> EPGPrograms { get; }
    IRepository<User> Users { get; }
    IRepository<ViewingHistory> ViewingHistories { get; }
    IRepository<UserPreference> UserPreferences { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
