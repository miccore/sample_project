using Miccore.Clean.Sample.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Miccore.Clean.Sample.Infrastructure.Tests.Fixtures;

/// <summary>
/// SQLite-based test database context for isolated integration tests.
/// Each test class gets its own isolated database instance.
/// </summary>
public class SqliteTestDbContext : DbContext
{
    public SqliteTestDbContext(DbContextOptions<SqliteTestDbContext> options) : base(options)
    {
    }

    public virtual DbSet<SampleEntity> Samples { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure SampleEntity
        modelBuilder.Entity<SampleEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.DeletedAt);
        });
    }
}
