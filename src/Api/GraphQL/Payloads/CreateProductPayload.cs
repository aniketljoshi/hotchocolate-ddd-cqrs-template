using ErrorOr;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Errors;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;

namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Payloads;

public sealed record CreateProductPayload(
    ProductDto? Product,
    IReadOnlyList<PayloadError> Errors)
{
    public static CreateProductPayload FromResult(ErrorOr<ProductDto> result)
    {
        return result.IsError
            ? new CreateProductPayload(null, result.Errors.Select(GraphQLErrorFactory.ToPayloadError).ToList())
            : new CreateProductPayload(result.Value, []);
    }
}
