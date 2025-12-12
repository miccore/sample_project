using Microsoft.EntityFrameworkCore.Storage;

namespace Miccore.Clean.Sample.Infrastructure.Persistance;

/// <summary>
/// Unit of Work implementation for managing database transactions.
/// Uses direct injection of repositories (CQRS pattern).
/// </summary>
/// <remarks>
/// Repositories are injected directly via constructor for:
/// - Better testability with Moq
/// - Explicit dependencies visible in DI container
/// - Clear separation in CQRS (Commands use UoW, Queries use repositories directly)
/// </remarks>
public class UnitOfWork(
    SampleApplicationDbContext context,
    ISampleRepository sampleRepository) : IUnitOfWork
{
    private readonly SampleApplicationDbContext _context = context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    /// <summary>
    /// Gets the Sample repository instance.
    /// </summary>
    public ISampleRepository Samples { get; } = sampleRepository;

    /// <summary>
    /// Saves all changes made in this unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("No transaction has been started. Call BeginTransactionAsync first.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await DisposeTransactionAsync();
        }
    }

    /// <summary>
    /// Disposes the current transaction.
    /// </summary>
    private async Task DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the unit of work and releases resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
        }
        _disposed = true;
    }
}
