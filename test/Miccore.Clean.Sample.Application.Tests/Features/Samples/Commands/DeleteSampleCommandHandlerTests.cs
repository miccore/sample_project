using AutoMapper;
using FluentAssertions;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.DeleteSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Tests.Fixtures;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Core.Interfaces;
using Miccore.Clean.Sample.Core.Repositories;
using Moq;

namespace Miccore.Clean.Sample.Application.Tests.Sample.Commands;

public class DeleteSampleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly IMapper _mapper;
    private readonly DeleteSampleCommandHandler _handler;

    public DeleteSampleCommandHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.Samples).Returns(_sampleRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mapper = TestMapperFactory.Create();
        _handler = new DeleteSampleCommandHandler(_unitOfWorkMock.Object, _mapper);
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
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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