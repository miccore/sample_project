using Moq;
using Miccore.Clean.Sample.Application.SampleFolder.Commands.CreateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Commands.CreateSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Repositories;
using Miccore.Clean.Sample.Core.Enums;
using Miccore.Clean.Sample.Core.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Miccore.Clean.Sample.Application.Features.Samples.Mappers;

namespace Miccore.Clean.Sample.Application.Tests.SampleFolder.Commands;

public class CreateSampleCommandHandlerTests
{
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly Mock<ILogger<CreateSampleCommandHandler>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly CreateSampleCommandHandler _handler;

    public CreateSampleCommandHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _loggerMock = new Mock<ILogger<CreateSampleCommandHandler>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<SampleMappingProfile>());
        _mapper = config.CreateMapper();
        
        _handler = new CreateSampleCommandHandler(_sampleRepositoryMock.Object, _mapper, _loggerMock.Object);
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
    public async Task Handle_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        // Arrange
        var command = null as CreateSampleCommand;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(command!, CancellationToken.None));
    }
}
