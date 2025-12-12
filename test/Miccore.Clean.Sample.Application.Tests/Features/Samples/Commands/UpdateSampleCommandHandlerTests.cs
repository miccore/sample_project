using AutoMapper;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.UpdateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Tests.Fixtures;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Core.Interfaces;
using Miccore.Clean.Sample.Core.Repositories;
using Moq;
using FluentAssertions;

namespace Miccore.Clean.Sample.Application.Tests.Sample.Commands;

public class UpdateSampleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly IMapper _mapper;
    private readonly UpdateSampleCommandHandler _handler;

    public UpdateSampleCommandHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.Samples).Returns(_sampleRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mapper = TestMapperFactory.Create();
        _handler = new UpdateSampleCommandHandler(_unitOfWorkMock.Object, _mapper);
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
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        // Arrange
        var command = null as UpdateSampleCommand;

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(command!, CancellationToken.None))
            .Should().ThrowAsync<ArgumentNullException>();
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
