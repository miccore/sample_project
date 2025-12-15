using System.Linq.Expressions;
using FluentAssertions;
using Miccore.Clean.Sample.Core.Configurations;
using Miccore.Clean.Sample.Core.Entities.Base;
using Miccore.Clean.Sample.Core.Interfaces;
using Miccore.Clean.Sample.Core.Repositories.Base;
using Miccore.Clean.Sample.Infrastructure.Caching;
using Miccore.Pagination;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Miccore.Clean.Sample.Infrastructure.Tests.Caching;

public class CachedRepositoryDecoratorTests
{
    private readonly Mock<IBaseRepository<TestEntity>> _innerRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<CachedRepositoryDecorator<TestEntity>>> _loggerMock;
    private readonly IOptions<CacheConfiguration> _cacheConfig;
    private readonly CachedRepositoryDecorator<TestEntity> _decorator;

    public CachedRepositoryDecoratorTests()
    {
        _innerRepositoryMock = new Mock<IBaseRepository<TestEntity>>();
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<CachedRepositoryDecorator<TestEntity>>>();
        _cacheConfig = Options.Create(new CacheConfiguration());
        _decorator = new CachedRepositoryDecorator<TestEntity>(
            _innerRepositoryMock.Object,
            _cacheServiceMock.Object,
            _loggerMock.Object,
            _cacheConfig);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenInnerRepositoryIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new CachedRepositoryDecorator<TestEntity>(
            null!,
            _cacheServiceMock.Object,
            _loggerMock.Object,
            _cacheConfig);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("innerRepository");
    }

    [Fact]
    public void Constructor_WhenCacheServiceIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new CachedRepositoryDecorator<TestEntity>(
            _innerRepositoryMock.Object,
            null!,
            _loggerMock.Object,
            _cacheConfig);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("cacheService");
    }

    [Fact]
    public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new CachedRepositoryDecorator<TestEntity>(
            _innerRepositoryMock.Object,
            _cacheServiceMock.Object,
            null!,
            _cacheConfig);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenCacheHit_ShouldReturnFromCache()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity { Id = id, Name = "Test" };
        _cacheServiceMock
            .Setup(c => c.GetAsync<TestEntity>(It.IsAny<string>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _decorator.GetByIdAsync(id);

        // Assert
        result.Should().Be(entity);
        _innerRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<string[]>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCacheMiss_ShouldGetFromRepositoryAndCache()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity { Id = id, Name = "Test" };

        _cacheServiceMock
            .Setup(c => c.GetAsync<TestEntity>(It.IsAny<string>()))
            .ReturnsAsync((TestEntity?)null);

        _innerRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<string[]>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _decorator.GetByIdAsync(id);

        // Assert
        result.Should().Be(entity);
        _innerRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<string[]>()), Times.Once);
        _cacheServiceMock.Verify(c => c.SetAsync(It.IsAny<string>(), entity, It.IsAny<TimeSpan?>()), Times.Once);
    }

    #endregion

    #region GetByParametersAsync Tests

    [Fact]
    public async Task GetByParametersAsync_ShouldCallInnerRepository()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };
        Expression<Func<TestEntity, bool>> expression = e => e.Name == "Test";

        _innerRepositoryMock
            .Setup(r => r.GetByParametersAsync(It.IsAny<Expression<Func<TestEntity, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync(entity);

        // Act
        var result = await _decorator.GetByParametersAsync(expression);

        // Assert
        result.Should().Be(entity);
        _innerRepositoryMock.Verify(
            r => r.GetByParametersAsync(It.IsAny<Expression<Func<TestEntity, bool>>>(), It.IsAny<string[]>()),
            Times.Once);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldCallInnerRepository()
    {
        // Arrange
        var paginationQuery = new PaginationQuery { Page = 1, Limit = 10 };
        var paginatedResult = new PaginationModel<TestEntity>
        {
            Items = new List<TestEntity> { new TestEntity { Id = Guid.NewGuid(), Name = "Test" } },
            CurrentPage = 1,
            TotalPages = 1,
            TotalItems = 1
        };

        _innerRepositoryMock
            .Setup(r => r.GetAllAsync(paginationQuery, It.IsAny<string[]>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _decorator.GetAllAsync(paginationQuery);

        // Assert
        result.Should().Be(paginatedResult);
        _innerRepositoryMock.Verify(r => r.GetAllAsync(paginationQuery, It.IsAny<string[]>()), Times.Once);
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ShouldAddToRepositoryAndCache()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "New Entity" };

        _innerRepositoryMock
            .Setup(r => r.AddAsync(entity))
            .ReturnsAsync(entity);

        // Act
        var result = await _decorator.AddAsync(entity);

        // Assert
        result.Should().Be(entity);
        _innerRepositoryMock.Verify(r => r.AddAsync(entity), Times.Once);
        _cacheServiceMock.Verify(c => c.SetAsync(It.IsAny<string>(), entity, It.IsAny<TimeSpan?>()), Times.Once);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldUpdateRepositoryAndRefreshCache()
    {
        // Arrange
        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Updated Entity" };

        _innerRepositoryMock
            .Setup(r => r.UpdateAsync(entity))
            .ReturnsAsync(entity);

        // Act
        var result = await _decorator.UpdateAsync(entity);

        // Assert
        result.Should().Be(entity);
        _innerRepositoryMock.Verify(r => r.UpdateAsync(entity), Times.Once);
        _cacheServiceMock.Verify(c => c.SetAsync(It.IsAny<string>(), entity, It.IsAny<TimeSpan?>()), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldDeleteFromRepositoryAndInvalidateCache()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity { Id = id, Name = "Deleted Entity" };

        _innerRepositoryMock
            .Setup(r => r.DeleteAsync(id))
            .ReturnsAsync(entity);

        // Act
        var result = await _decorator.DeleteAsync(id);

        // Assert
        result.Should().Be(entity);
        _innerRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Once);
    }

    #endregion

    #region DeleteHardAsync Tests

    [Fact]
    public async Task DeleteHardAsync_ShouldDeleteFromRepositoryAndInvalidateAllCache()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> expression = e => e.Name == "Test";

        _innerRepositoryMock
            .Setup(r => r.DeleteHardAsync(It.IsAny<Expression<Func<TestEntity, bool>>>()))
            .Returns(Task.CompletedTask);

        // Act
        await _decorator.DeleteHardAsync(expression);

        // Assert
        _innerRepositoryMock.Verify(
            r => r.DeleteHardAsync(It.IsAny<Expression<Func<TestEntity, bool>>>()),
            Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveByPatternAsync(It.IsAny<string>()), Times.Once);
    }

    #endregion

    #region GetAllByParametersAsync Tests

    [Fact]
    public async Task GetAllByParametersAsync_ShouldCallInnerRepository()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> expression = e => e.Name == "Test";
        var entities = new List<TestEntity> { new TestEntity { Id = Guid.NewGuid(), Name = "Test" } };

        _innerRepositoryMock
            .Setup(r => r.GetAllByParametersAsync(It.IsAny<Expression<Func<TestEntity, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync(entities);

        // Act
        var result = await _decorator.GetAllByParametersAsync(expression);

        // Assert
        result.Should().BeEquivalentTo(entities);
        _innerRepositoryMock.Verify(
            r => r.GetAllByParametersAsync(It.IsAny<Expression<Func<TestEntity, bool>>>(), It.IsAny<string[]>()),
            Times.Once);
    }

    #endregion

    #region GetAllByParametersPaginatedAsync Tests

    [Fact]
    public async Task GetAllByParametersPaginatedAsync_ShouldCallInnerRepository()
    {
        // Arrange
        var paginationQuery = new PaginationQuery { Page = 1, Limit = 10 };
        Expression<Func<TestEntity, bool>> expression = e => e.Name == "Test";
        var paginatedResult = new PaginationModel<TestEntity>
        {
            Items = new List<TestEntity> { new TestEntity { Id = Guid.NewGuid(), Name = "Test" } },
            CurrentPage = 1,
            TotalPages = 1,
            TotalItems = 1
        };

        _innerRepositoryMock
            .Setup(r => r.GetAllByParametersPaginatedAsync(paginationQuery, It.IsAny<Expression<Func<TestEntity, bool>>>(), It.IsAny<string[]>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _decorator.GetAllByParametersPaginatedAsync(paginationQuery, expression);

        // Assert
        result.Should().Be(paginatedResult);
        _innerRepositoryMock.Verify(
            r => r.GetAllByParametersPaginatedAsync(paginationQuery, It.IsAny<Expression<Func<TestEntity, bool>>>(), It.IsAny<string[]>()),
            Times.Once);
    }

    #endregion

    // Test entity
    public class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}
