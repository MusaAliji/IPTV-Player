using IPTV.Core.Entities;
using IPTV.Core.Interfaces;
using IPTV.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace IPTV.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IPTVDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRepository<Content>? _contents;
    private IRepository<Channel>? _channels;
    private IRepository<EPGProgram>? _epgPrograms;
    private IRepository<User>? _users;
    private IRepository<ViewingHistory>? _viewingHistories;
    private IRepository<UserPreference>? _userPreferences;

    public UnitOfWork(IPTVDbContext context)
    {
        _context = context;
    }

    public IRepository<Content> Contents =>
        _contents ??= new Repository<Content>(_context);

    public IRepository<Channel> Channels =>
        _channels ??= new Repository<Channel>(_context);

    public IRepository<EPGProgram> EPGPrograms =>
        _epgPrograms ??= new Repository<EPGProgram>(_context);

    public IRepository<User> Users =>
        _users ??= new Repository<User>(_context);

    public IRepository<ViewingHistory> ViewingHistories =>
        _viewingHistories ??= new Repository<ViewingHistory>(_context);

    public IRepository<UserPreference> UserPreferences =>
        _userPreferences ??= new Repository<UserPreference>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
