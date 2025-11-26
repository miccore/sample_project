using Miccore.Clean.Sample.Application.Sample.Commands.DeleteSample;
using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Core.Repositories;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Miccore.Clean.Sample.Application.Tests.Sample.Commands;

public class DeleteSampleCommandHandlerTests
{
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly Mock<ILogger<DeleteSampleCommandHandler>> _loggerMock;
    private readonly DeleteSampleCommandHandler _handler;

    public DeleteSampleCommandHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _loggerMock = new Mock<ILogger<DeleteSampleCommandHandler>>();
        _handler = new DeleteSampleCommandHandler(_sampleRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteSampleAndReturnResponse()
    {
        // Arrange
        var command = new DeleteSampleCommand(Guid.NewGuid());
        var sampleEntity = new SampleEntity { Id = command.Id, Name = "Test Sample" };
        var sampleResponse = new SampleResponse { Id = sampleEntity.Id, Name = "Test Sample" };

        _sampleRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(sampleEntity);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sampleResponse.Id);
        result.Name.Should().Be(sampleResponse.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenSampleDoesNotExist()
    {
        // Arrange
        var command = new DeleteSampleCommand(Guid.NewGuid());

        _sampleRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new NotFoundException("Sample not found"));

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage("Sample not found");
    }
}