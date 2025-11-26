using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Miccore.Clean.Sample.Infrastructure.Tests.Fixtures;

/// <summary>
/// Fixture for SQLite in-memory database tests.
/// Creates an isolated database instance per test class.
/// Implements IDisposable to ensure proper cleanup.
/// </summary>
public class SqliteTestFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    private bool _disposed;

    /// <summary>
    /// Gets the DbContext for this test fixture.
    /// </summary>
    public SqliteTestDbContext Context { get; }

    /// <summary>
    /// Gets the DbContextOptions used to create the context.
    /// </summary>
    public DbContextOptions<SqliteTestDbContext> Options { get; }

    public SqliteTestFixture()
    {
        // Create and open a connection. This keeps the in-memory database alive.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        Options = new DbContextOptionsBuilder<SqliteTestDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        Context = new SqliteTestDbContext(Options);

        // Ensure database is created
        Context.Database.EnsureCreated();
    }

    /// <summary>
    /// Creates a new DbContext instance using the same connection.
    /// Useful for testing scenarios that require a fresh context.
    /// </summary>
    public SqliteTestDbContext CreateContext()
    {
        return new SqliteTestDbContext(Options);
    }

    /// <summary>
    /// Resets the database by deleting and recreating all tables.
    /// Call this at the beginning of each test for complete isolation.
    /// </summary>
    public void ResetDatabase()
    {
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Context.Dispose();
                _connection.Dispose();
            }
            _disposed = true;
        }
    }
}
