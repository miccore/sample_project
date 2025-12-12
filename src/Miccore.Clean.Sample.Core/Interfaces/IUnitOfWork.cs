using Miccore.Clean.Sample.Core.Repositories;

namespace Miccore.Clean.Sample.Core.Interfaces;

/// <summary>
/// Unit of Work interface for managing database transactions.
/// Provides a single point of control for committing changes and managing transactions.
/// </summary>
/// <remarks>
/// In CQRS architecture:
/// - Command Handlers should use IUnitOfWork for write operations
/// - Query Handlers should inject repositories directly for read operations
/// </remarks>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the Sample repository instance.
    /// </summary>
    ISampleRepository Samples { get; }

    /// <summary>
    /// Saves all changes made in this unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// Use this for operations that require explicit transaction control.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
