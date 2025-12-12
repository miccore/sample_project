using AutoMapper;
using FluentAssertions;
using Miccore.Clean.Sample.Application.Features.Samples.Queries.GetAllSamples;
using Miccore.Clean.Sample.Application.Features.Samples.Responses;
using Miccore.Clean.Sample.Application.Tests.Fixtures;
using Miccore.Clean.Sample.Core.Entities;
using Miccore.Clean.Sample.Core.Repositories;
using Miccore.Pagination;
using Moq;

namespace Miccore.Clean.Sample.Application.Tests.Sample.Queries;

public class GetAllSamplesQueryHandlerTests
{
    private readonly Mock<ISampleRepository> _sampleRepositoryMock;
    private readonly IMapper _mapper;
    private readonly GetAllSamplesQueryHandler _handler;

    public GetAllSamplesQueryHandlerTests()
    {
        _sampleRepositoryMock = new Mock<ISampleRepository>();
        _mapper = TestMapperFactory.Create();
        _handler = new GetAllSamplesQueryHandler(_sampleRepositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfSampleResponses()
    {
        // Arrange
        var query = new GetAllSamplesQuery(new PaginationQuery());  
        query.Query.Paginate = false;
        query.Query.Page = 1;
        query.Query.Limit = 10;
        var sampleEntities = new PaginationModel<SampleEntity>{
            Items = new List<SampleEntity>
            {
                new SampleEntity { Id = Guid.NewGuid(), Name = "Sample 1" },
                new SampleEntity { Id = Guid.NewGuid(), Name = "Sample 2" }
            }
        };

        var response = _mapper.Map<PaginationModel<SampleResponse>>(sampleEntities);

        _sampleRepositoryMock.Setup(repo => repo.GetAllAsync(query.Query))
            .ReturnsAsync(sampleEntities);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalItems.Should().Be(response.TotalItems);
        result.Items.Should().HaveCount(response.Items.Count);
        result.Items[0].Id.Should().Be(response.Items[0].Id);
        result.Items[0].Name.Should().Be(response.Items[0].Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoSamplesExist()
    {
        // Arrange
        var query = new GetAllSamplesQuery(new PaginationQuery());  
        query.Query.Paginate = false;
        query.Query.Page = 1;
        query.Query.Limit = 10;
        var sampleEntities = new PaginationModel<SampleEntity>{
            Items = new List<SampleEntity>()
        };
        
        _sampleRepositoryMock.Setup(repo => repo.GetAllAsync(query.Query))
            .ReturnsAsync(sampleEntities);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }
}
