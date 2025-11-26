using FluentAssertions;
using Miccore.Clean.Sample.Core.Configurations;

namespace Miccore.Clean.Sample.Core.Tests.Configurations;

public class CacheConfigurationTests
{
    [Fact]
    public void SectionName_ShouldBeCache()
    {
        // Assert
        CacheConfiguration.SectionName.Should().Be("Cache");
    }

    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        // Arrange
        var config = new CacheConfiguration();

        // Assert
        config.DefaultExpirationMinutes.Should().Be(60);
        config.RepositoryExpirationMinutes.Should().Be(30);
        config.SlidingExpirationMinutes.Should().Be(5);
        config.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void DefaultExpiration_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        var config = new CacheConfiguration { DefaultExpirationMinutes = 120 };

        // Act & Assert
        config.DefaultExpiration.Should().Be(TimeSpan.FromMinutes(120));
    }

    [Fact]
    public void RepositoryExpiration_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        var config = new CacheConfiguration { RepositoryExpirationMinutes = 45 };

        // Act & Assert
        config.RepositoryExpiration.Should().Be(TimeSpan.FromMinutes(45));
    }

    [Fact]
    public void SlidingExpiration_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        var config = new CacheConfiguration { SlidingExpirationMinutes = 10 };

        // Act & Assert
        config.SlidingExpiration.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var config = new CacheConfiguration();

        // Act
        config.DefaultExpirationMinutes = 90;
        config.RepositoryExpirationMinutes = 60;
        config.SlidingExpirationMinutes = 15;
        config.IsEnabled = false;

        // Assert
        config.DefaultExpirationMinutes.Should().Be(90);
        config.RepositoryExpirationMinutes.Should().Be(60);
        config.SlidingExpirationMinutes.Should().Be(15);
        config.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void DefaultExpiration_WithDefaultValue_ShouldBeOneHour()
    {
        // Arrange
        var config = new CacheConfiguration();

        // Act & Assert
        config.DefaultExpiration.Should().Be(TimeSpan.FromHours(1));
    }

    [Fact]
    public void RepositoryExpiration_WithDefaultValue_ShouldBe30Minutes()
    {
        // Arrange
        var config = new CacheConfiguration();

        // Act & Assert
        config.RepositoryExpiration.Should().Be(TimeSpan.FromMinutes(30));
    }

    [Fact]
    public void SlidingExpiration_WithDefaultValue_ShouldBe5Minutes()
    {
        // Arrange
        var config = new CacheConfiguration();

        // Act & Assert
        config.SlidingExpiration.Should().Be(TimeSpan.FromMinutes(5));
    }
}
