using FluentAssertions;
using Miccore.Clean.Sample.Core.Configurations;

namespace Miccore.Clean.Sample.Core.Tests.Configurations;

public class DatabaseConfigurationTests
{
    [Fact]
    public void SectionName_ShouldBeDatabase()
    {
        // Assert
        DatabaseConfiguration.SectionName.Should().Be("Database");
    }

    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        // Arrange
        var config = new DatabaseConfiguration();

        // Assert
        config.Server.Should().BeEmpty();
        config.Port.Should().Be(3306);
        config.Name.Should().BeEmpty();
        config.User.Should().BeEmpty();
        config.Password.Should().BeEmpty();
    }

    [Fact]
    public void GetConnectionString_ShouldReturnValidConnectionString()
    {
        // Arrange
        var config = new DatabaseConfiguration
        {
            Server = "localhost",
            Port = 3306,
            Name = "test_db",
            User = "root",
            Password = "password123"
        };

        // Act
        var connectionString = config.GetConnectionString();

        // Assert
        connectionString.Should().Be("server=localhost;port=3306;database=test_db;user=root;password=password123");
    }

    [Fact]
    public void GetConnectionString_WithCustomPort_ShouldIncludePort()
    {
        // Arrange
        var config = new DatabaseConfiguration
        {
            Server = "db.example.com",
            Port = 3307,
            Name = "production_db",
            User = "admin",
            Password = "secret"
        };

        // Act
        var connectionString = config.GetConnectionString();

        // Assert
        connectionString.Should().Contain("port=3307");
        connectionString.Should().Contain("server=db.example.com");
    }

    [Fact]
    public void GetConnectionString_WithEmptyPassword_ShouldStillWork()
    {
        // Arrange
        var config = new DatabaseConfiguration
        {
            Server = "localhost",
            Port = 3306,
            Name = "test_db",
            User = "root",
            Password = ""
        };

        // Act
        var connectionString = config.GetConnectionString();

        // Assert
        connectionString.Should().Be("server=localhost;port=3306;database=test_db;user=root;password=");
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var config = new DatabaseConfiguration();

        // Act
        config.Server = "myserver";
        config.Port = 5432;
        config.Name = "mydb";
        config.User = "myuser";
        config.Password = "mypass";

        // Assert
        config.Server.Should().Be("myserver");
        config.Port.Should().Be(5432);
        config.Name.Should().Be("mydb");
        config.User.Should().Be("myuser");
        config.Password.Should().Be("mypass");
    }
}
