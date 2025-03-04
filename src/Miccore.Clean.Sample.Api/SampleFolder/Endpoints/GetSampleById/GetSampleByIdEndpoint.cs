using Miccore.Clean.Sample.Api.Sample.Mappers;
using Miccore.Clean.Sample.Application.Sample.Queries.GetSampleById;

namespace Miccore.Clean.Sample.Api.Sample.Endpoints.GetSampleById;

public sealed class GetSampleByIdEndpoint(IMediator _mediator) : Endpoint<GetSampleByIdRequest, ApiResponse<GetSampleByIdSampleResponse>>
{
    private static readonly string _route = "/sample/{id}";
    private static readonly AutoMapper.IMapper Mapper = SampleEndpointMapper.Mapper;
    
    public override void Configure()
    {
        Get(_route);
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetSampleByIdRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (ValidationFailed)
            {
                var failures = ValidationFailures.Select(x => $"{x.PropertyName}: {x.ErrorMessage}").ToList();
                throw new ValidatorException(string.Join("\n", failures));
            }

            var response = await _mediator.Send(Mapper.Map<GetSampleByIdQuery>(request), cancellationToken);

            await SendAsync(ApiResponse<GetSampleByIdSampleResponse>.Success(
                Mapper.Map<GetSampleByIdSampleResponse>(response)
            ), cancellation: cancellationToken);
        }
        catch (NotFoundException notFound)
        {
            await SendAsync(ApiResponse<GetSampleByIdSampleResponse>.Error(HttpStatusCode.NotFound, notFound.Message), (int) HttpStatusCode.NotFound, cancellationToken);
        }
        catch (ValidatorException invalid)
        {
            await SendAsync(ApiResponse<GetSampleByIdSampleResponse>.Error(HttpStatusCode.BadRequest, invalid.Message), (int) HttpStatusCode.BadRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            await SendAsync(ApiResponse<GetSampleByIdSampleResponse>.Error(HttpStatusCode.InternalServerError, ex.Message), (int) HttpStatusCode.InternalServerError, cancellationToken);
        }
    }
}
