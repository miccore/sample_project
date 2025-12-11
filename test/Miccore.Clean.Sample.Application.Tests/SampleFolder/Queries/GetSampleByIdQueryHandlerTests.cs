using Miccore.Clean.Sample.Application.SampleFolder.Queries.GetSampleById;
using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Core.Repositories;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Miccore.Clean.Sample.Application.Features.Samples.Mappers;

namespace Miccore.Clean.Sample.Application.Tests.SampleFolder.Queries;

public class GetSampleByIdQueryHandlerTests
{
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly Mock<ILogger<GetSampleByIdQueryHandler>> _loggerMock;
    private readonly IMapper _mapper;
    private readonly GetSampleByIdQueryHandler _handler;

    public GetSampleByIdQueryHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _loggerMock = new Mock<ILogger<GetSampleByIdQueryHandler>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<SampleMappingProfile>());
        _mapper = config.CreateMapper();
        
        _handler = new GetSampleByIdQueryHandler(_sampleRepositoryMock.Object, _mapper, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSampleResponse_WhenSampleExists()
    {
        // Arrange
        var query = new GetSampleQuery(Guid.NewGuid());
        var sampleEntity = new SampleEntity { Id = query.Id, Name = "Test Sample" };
        var sampleResponse = new SampleResponse { Id = sampleEntity.Id, Name = "Test Sample" };

        _sampleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(sampleEntity);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sampleResponse.Id);
        result.Name.Should().Be(sampleResponse.Name);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenSampleDoesNotExist()
    {
        // Arrange
        var query = new GetSampleQuery(Guid.NewGuid());

        _sampleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new NotFoundException("Sample not found"));

        // Act & Assert
        await FluentActions.Invoking(() => _handler.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}
