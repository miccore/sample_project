using AutoMapper;
using Moq;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.CreateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Tests.Fixtures;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Interfaces;
using Miccore.Clean.Sample.Core.Repositories;
using Miccore.Clean.Sample.Core.Enums;
using Miccore.Clean.Sample.Core.Extensions;
using FluentAssertions;

namespace Miccore.Clean.Sample.Application.Tests.Sample.Commands;

public class CreateSampleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly IMapper _mapper;
    private readonly CreateSampleCommandHandler _handler;

    public CreateSampleCommandHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.Samples).Returns(_sampleRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mapper = TestMapperFactory.Create();
        _handler = new CreateSampleCommandHandler(_unitOfWorkMock.Object, _mapper);
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
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        // Arrange
        var command = null as CreateSampleCommand;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(command!, CancellationToken.None));
    }
}
