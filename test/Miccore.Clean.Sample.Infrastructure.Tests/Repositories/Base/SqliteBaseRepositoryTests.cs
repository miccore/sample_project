using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Infrastructure.Tests.Fixtures;
using Miccore.Pagination.Model;
using FluentAssertions;

namespace Miccore.Clean.Sample.Infrastructure.Tests.Repositories.Base;

/// <summary>
/// Integration tests for repository operations using SQLite in-memory database.
/// Each test gets a fresh, isolated database instance.
/// </summary>
public class SqliteBaseRepositoryTests : IDisposable
{
    private readonly SqliteTestFixture _fixture;
    private readonly SqliteTestRepository<SampleEntity> _repository;

    public SqliteBaseRepositoryTests()
    {
        _fixture = new SqliteTestFixture();
        _repository = new SqliteTestRepository<SampleEntity>(_fixture.Context);
    }

    public void Dispose()
    {
        _fixture.Dispose();
        GC.SuppressFinalize(this);
    }

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        var entity = new SampleEntity { Name = "Test Entity" };

        // Act
        var result = await _repository.AddAsync(entity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be("Test Entity");
        
        // Verify in database
        var dbEntity = await _fixture.Context.Samples.FindAsync(result.Id);
        dbEntity.Should().NotBeNull();
        dbEntity!.Name.Should().Be("Test Entity");
    }

    [Fact]
    public async Task AddAsync_ShouldSetCreatedAt()
    {
        // Arrange
        var entity = new SampleEntity { Name = "Test" };
        var beforeAdd = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Act
        var result = await _repository.AddAsync(entity);
        var afterAdd = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Assert
        result.CreatedAt.Should().BeGreaterThanOrEqualTo(beforeAdd);
        result.CreatedAt.Should().BeLessThanOrEqualTo(afterAdd);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
    {
        // Arrange
        var entity = new SampleEntity { Name = "Test" };
        await _repository.AddAsync(entity);

        // Act
        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _repository.GetByIdAsync(nonExistentId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenSoftDeleted()
    {
        // Arrange
        var entity = new SampleEntity { Name = "Test" };
        await _repository.AddAsync(entity);
        await _repository.DeleteAsync(entity.Id);

        // Act
        Func<Task> act = async () => await _repository.GetByIdAsync(entity.Id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedEntities()
    {
        // Arrange
        await _repository.AddAsync(new SampleEntity { Name = "Entity 1" });
        await _repository.AddAsync(new SampleEntity { Name = "Entity 2" });
        await _repository.AddAsync(new SampleEntity { Name = "Entity 3" });
        
        var query = new PaginationQuery { page = 1, limit = 10 };

        // Act
        var result = await _repository.GetAllAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalItems.Should().Be(3);
    }

    [Fact]
    public async Task GetAllAsync_ShouldExcludeSoftDeletedEntities()
    {
        // Arrange
        var entity1 = new SampleEntity { Name = "Active" };
        var entity2 = new SampleEntity { Name = "Deleted" };
        await _repository.AddAsync(entity1);
        await _repository.AddAsync(entity2);
        await _repository.DeleteAsync(entity2.Id);
        
        var query = new PaginationQuery { page = 1, limit = 10 };

        // Act
        var result = await _repository.GetAllAsync(query);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.Should().OnlyContain(x => x.Name == "Active");
    }

    [Fact]
    public async Task GetAllAsync_ShouldPaginateCorrectly()
    {
        // Arrange
        for (int i = 1; i <= 15; i++)
        {
            await _repository.AddAsync(new SampleEntity { Name = $"Entity {i}" });
        }
        
        var query = new PaginationQuery { page = 2, limit = 5 };

        // Act
        var result = await _repository.GetAllAsync(query);

        // Assert
        result.Items.Should().HaveCount(5);
        result.TotalItems.Should().Be(15);
        result.CurrentPage.Should().Be(2);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteEntity()
    {
        // Arrange
        var entity = new SampleEntity { Name = "Test" };
        await _repository.AddAsync(entity);

        // Act
        var result = await _repository.DeleteAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result.DeletedAt.Should().NotBeNull();
        result.DeletedAt.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _repository.DeleteAsync(nonExistentId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenAlreadyDeleted()
    {
        // Arrange
        var entity = new SampleEntity { Name = "Test" };
        await _repository.AddAsync(entity);
        await _repository.DeleteAsync(entity.Id);

        // Act
        Func<Task> act = async () => await _repository.DeleteAsync(entity.Id);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region DeleteHardAsync Tests

    [Fact]
    public async Task DeleteHardAsync_ShouldPermanentlyDeleteEntities()
    {
        // Arrange
        var entity = new SampleEntity { Name = "ToDelete" };
        await _repository.AddAsync(entity);

        // Act
        await _repository.DeleteHardAsync(x => x.Name == "ToDelete");

        // Assert
        var dbEntity = await _fixture.Context.Samples.FindAsync(entity.Id);
        dbEntity.Should().BeNull();
    }

    [Fact]
    public async Task DeleteHardAsync_ShouldOnlyDeleteMatchingEntities()
    {
        // Arrange
        await _repository.AddAsync(new SampleEntity { Name = "Keep" });
        await _repository.AddAsync(new SampleEntity { Name = "Delete" });
        await _repository.AddAsync(new SampleEntity { Name = "Delete" });

        // Act
        await _repository.DeleteHardAsync(x => x.Name == "Delete");

        // Assert
        var remaining = _fixture.Context.Samples.ToList();
        remaining.Should().HaveCount(1);
        remaining.First().Name.Should().Be("Keep");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var entity = new SampleEntity { Name = "Original" };
        await _repository.AddAsync(entity);
        
        entity.Name = "Updated";

        // Act
        var result = await _repository.UpdateAsync(entity);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated");
        result.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenNotExists()
    {
        // Arrange
        var entity = new SampleEntity { Id = Guid.NewGuid(), Name = "NonExistent" };

        // Act
        Func<Task> act = async () => await _repository.UpdateAsync(entity);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetByParametersAsync Tests

    [Fact]
    public async Task GetByParametersAsync_ShouldReturnMatchingEntity()
    {
        // Arrange
        await _repository.AddAsync(new SampleEntity { Name = "Target" });
        await _repository.AddAsync(new SampleEntity { Name = "Other" });

        // Act
        var result = await _repository.GetByParametersAsync(x => x.Name == "Target");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Target");
    }

    [Fact]
    public async Task GetByParametersAsync_ShouldThrowNotFoundException_WhenNoMatch()
    {
        // Arrange
        await _repository.AddAsync(new SampleEntity { Name = "Other" });

        // Act
        Func<Task> act = async () => await _repository.GetByParametersAsync(x => x.Name == "NonExistent");

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    #endregion

    #region GetAllByParametersAsync Tests

    [Fact]
    public async Task GetAllByParametersAsync_ShouldReturnMatchingEntities()
    {
        // Arrange
        await _repository.AddAsync(new SampleEntity { Name = "Match" });
        await _repository.AddAsync(new SampleEntity { Name = "Match" });
        await _repository.AddAsync(new SampleEntity { Name = "Other" });

        // Act
        var result = await _repository.GetAllByParametersAsync(x => x.Name == "Match");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(x => x.Name == "Match");
    }

    [Fact]
    public async Task GetAllByParametersAsync_ShouldReturnEmptyList_WhenNoMatch()
    {
        // Arrange
        await _repository.AddAsync(new SampleEntity { Name = "Other" });

        // Act
        var result = await _repository.GetAllByParametersAsync(x => x.Name == "NonExistent");

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetAllByParametersPaginatedAsync Tests

    [Fact]
    public async Task GetAllByParametersPaginatedAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            await _repository.AddAsync(new SampleEntity { Name = i % 2 == 0 ? "Even" : "Odd" });
        }
        
        var query = new PaginationQuery { page = 1, limit = 3 };

        // Act
        var result = await _repository.GetAllByParametersPaginatedAsync(query, x => x.Name == "Even");

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalItems.Should().Be(5);
        result.Items.Should().OnlyContain(x => x.Name == "Even");
    }

    #endregion
}
