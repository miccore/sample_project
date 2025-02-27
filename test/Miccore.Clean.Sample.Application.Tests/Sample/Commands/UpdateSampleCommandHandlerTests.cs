using Miccore.Clean.Sample.Application.Sample.Commands.UpdateSample;
using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Core.Repositories;
using Moq;
using FluentAssertions;

namespace Miccore.Clean.Sample.Application.Tests.Sample.Commands;

public class UpdateSampleCommandHandlerTests
{
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly UpdateSampleCommandHandler _handler;

    public UpdateSampleCommandHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _handler = new UpdateSampleCommandHandler(_sampleRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateSampleAndReturnResponse()
    {
        // Arrange
        var command = new UpdateSampleCommand { Id = Guid.NewGuid(), Name = "Updated Sample" };
        var sampleEntity = new SampleEntity { Id = command.Id, Name = "Updated Sample" };
        var sampleResponse = new SampleResponse { Id = sampleEntity.Id, Name = "Updated Sample" };

        _sampleRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<SampleEntity>()))
            .ReturnsAsync(sampleEntity);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sampleResponse.Id);
        result.Name.Should().Be(sampleResponse.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowApplicationException_WhenMapperFails()
    {
        // Arrange
        var command = null as UpdateSampleCommand;

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(command!, CancellationToken.None))
            .Should().ThrowAsync<ApplicationException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenSampleDoesNotExist()
    {
        // Arrange
        var command = new UpdateSampleCommand { Id = Guid.NewGuid(), Name = "Updated Sample" };

        _sampleRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<SampleEntity>()))
            .ThrowsAsync(new NotFoundException("Sample not found"));

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}
