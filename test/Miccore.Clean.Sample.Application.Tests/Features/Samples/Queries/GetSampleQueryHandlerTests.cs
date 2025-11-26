using AutoMapper;
using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetSample;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Tests.Fixtures;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Exceptions;
using Miccore.Clean.Sample.Core.Repositories;
using Moq;
using FluentAssertions;

namespace Miccore.Clean.Sample.Application.Tests.Sample.Queries;

public class GetSampleQueryHandlerTests
{
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly IMapper _mapper;
    private readonly GetSampleQueryHandler _handler;

    public GetSampleQueryHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _mapper = TestMapperFactory.Create();
        _handler = new GetSampleQueryHandler(_sampleRepositoryMock.Object, _mapper);
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
