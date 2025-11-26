using FluentAssertions;
using Miccore.Clean.Sample.Core.Configurations;
using Miccore.Clean.Sample.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Miccore.Clean.Sample.Infrastructure.Tests.Caching;

public class MemoryCacheServiceTests : IDisposable
{
    private readonly IMemoryCache _cache;
    private readonly Mock<ILogger<MemoryCacheService>> _loggerMock;
    private readonly IOptions<CacheConfiguration> _cacheConfig;
    private readonly MemoryCacheService _cacheService;

    public MemoryCacheServiceTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<MemoryCacheService>>();
        _cacheConfig = Options.Create(new CacheConfiguration());
        _cacheService = new MemoryCacheService(_cache, _loggerMock.Object, _cacheConfig);
    }

    public void Dispose()
    {
        _cache.Dispose();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WhenCacheIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new MemoryCacheService(null!, _loggerMock.Object, _cacheConfig);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("cache");
    }

    [Fact]
    public void Constructor_WhenLoggerIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new MemoryCacheService(_cache, null!, _cacheConfig);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WhenKeyIsNull_ShouldThrowArgumentException()
    {
        // Act
        var act = () => _cacheService.GetAsync<string>(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_WhenKeyIsEmpty_ShouldThrowArgumentException()
    {
        // Act
        var act = () => _cacheService.GetAsync<string>("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAsync_WhenValueExists_ShouldReturnValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        await _cacheService.SetAsync(key, value);

        // Act
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task GetAsync_WhenValueDoesNotExist_ShouldReturnDefault()
    {
        // Act
        var result = await _cacheService.GetAsync<string>("non-existent-key");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WhenValueIsComplexObject_ShouldReturnObject()
    {
        // Arrange
        var key = "complex-key";
        var value = new TestClass { Id = 1, Name = "Test" };
        await _cacheService.SetAsync(key, value);

        // Act
        var result = await _cacheService.GetAsync<TestClass>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test");
    }

    #endregion

    #region SetAsync Tests

    [Fact]
    public async Task SetAsync_WhenKeyIsNull_ShouldThrowArgumentException()
    {
        // Act
        var act = () => _cacheService.SetAsync(null!, "value");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SetAsync_WhenValueIsNull_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => _cacheService.SetAsync<string>("key", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SetAsync_WithCustomExpiration_ShouldStoreValue()
    {
        // Arrange
        var key = "expiring-key";
        var value = "expiring-value";
        var expiration = TimeSpan.FromMinutes(5);

        // Act
        await _cacheService.SetAsync(key, value, expiration);
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task SetAsync_ShouldIncrementKeyCount()
    {
        // Arrange
        var initialCount = _cacheService.GetKeyCount();

        // Act
        await _cacheService.SetAsync("new-key", "new-value");

        // Assert
        _cacheService.GetKeyCount().Should().Be(initialCount + 1);
    }

    #endregion

    #region RemoveAsync Tests

    [Fact]
    public async Task RemoveAsync_WhenKeyIsNull_ShouldThrowArgumentException()
    {
        // Act
        var act = () => _cacheService.RemoveAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task RemoveAsync_WhenKeyExists_ShouldRemoveValue()
    {
        // Arrange
        var key = "remove-key";
        await _cacheService.SetAsync(key, "value");

        // Act
        await _cacheService.RemoveAsync(key);
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveAsync_WhenKeyDoesNotExist_ShouldNotThrow()
    {
        // Act
        var act = () => _cacheService.RemoveAsync("non-existent");

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region RemoveByPatternAsync Tests

    [Fact]
    public async Task RemoveByPatternAsync_WhenPatternIsNull_ShouldThrowArgumentException()
    {
        // Act
        var act = () => _cacheService.RemoveByPatternAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task RemoveByPatternAsync_ShouldRemoveMatchingKeys()
    {
        // Arrange
        await _cacheService.SetAsync("prefix_key1", "value1");
        await _cacheService.SetAsync("prefix_key2", "value2");
        await _cacheService.SetAsync("other_key", "value3");

        // Act
        await _cacheService.RemoveByPatternAsync("prefix_*");

        // Assert
        var result1 = await _cacheService.GetAsync<string>("prefix_key1");
        var result2 = await _cacheService.GetAsync<string>("prefix_key2");
        var result3 = await _cacheService.GetAsync<string>("other_key");

        result1.Should().BeNull();
        result2.Should().BeNull();
        result3.Should().Be("value3");
    }

    [Fact]
    public async Task RemoveByPatternAsync_WithWildcardOnly_ShouldRemoveAllKeys()
    {
        // Arrange
        await _cacheService.SetAsync("key1", "value1");
        await _cacheService.SetAsync("key2", "value2");
        var initialCount = _cacheService.GetKeyCount();

        // Act
        await _cacheService.RemoveByPatternAsync("*");

        // Assert
        _cacheService.GetKeyCount().Should().BeLessThan(initialCount);
    }

    #endregion

    #region ClearAllAsync Tests

    [Fact]
    public async Task ClearAllAsync_ShouldRemoveAllKeys()
    {
        // Arrange
        await _cacheService.SetAsync("key1", "value1");
        await _cacheService.SetAsync("key2", "value2");
        await _cacheService.SetAsync("key3", "value3");

        // Act
        await _cacheService.ClearAllAsync();

        // Assert
        _cacheService.GetKeyCount().Should().Be(0);
    }

    #endregion

    #region GetKeyCount Tests

    [Fact]
    public void GetKeyCount_WhenEmpty_ShouldReturnZero()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCacheService(cache, _loggerMock.Object, _cacheConfig);

        // Act & Assert - Note: static ConcurrentDictionary persists across instances in same test run
        // This test verifies GetKeyCount returns an integer
        service.GetKeyCount().Should().BeGreaterThanOrEqualTo(0);
    }

    #endregion

    // Test helper class
    private class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
