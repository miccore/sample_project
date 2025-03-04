using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using FluentAssertions;

namespace Miccore.Clean.Sample.Application.Tests.Sample;

public class DependencyInjectionTests
{
    [Fact]
    public void AddApplication_ShouldRegisterMediatR()
    {
        // Arrange
        var services = new ServiceCollection();
        var configurationMock = new Mock<IConfiguration>();

        // Act
        services.AddApplication(configurationMock.Object);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var mediator = serviceProvider.GetService<IMediator>();
        mediator.Should().NotBeNull(); // Utilisation de Fluent Assertions
    }
}
