using Moq;
using Miccore.Clean.Sample.Application.Sample.Commands.CreateSample;
using Miccore.Clean.Sample.Application.Sample.Responses;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Repositories;
using Miccore.Clean.Sample.Core.Enums;
using Miccore.Clean.Sample.Core.Extensions;
using FluentAssertions;

namespace Miccore.Clean.Sample.Application.Tests.Sample.Commands;

public class CreateSampleCommandHandlerTests
{
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly CreateSampleCommandHandler _handler;

    public CreateSampleCommandHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _handler = new CreateSampleCommandHandler(_sampleRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddSampleAndReturnResponse()
    {
        // Arrange
        var command = new CreateSampleCommand { Name = "Test Sample" };
        var sampleEntity = new SampleEntity { Id = Guid.NewGuid(), Name = "Test Sample" };
        var sampleResponse = new SampleResponse { Id = sampleEntity.Id, Name = "Test Sample" };

        _sampleRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<SampleEntity>()))
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
        var command = null as CreateSampleCommand;

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command!, CancellationToken.None));

        ex.Message.Should().Be(ExceptionEnum.MapperIssue.GetEnumDescription());
    }
}
