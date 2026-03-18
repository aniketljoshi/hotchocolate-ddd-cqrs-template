using ErrorOr;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Errors;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;

namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Payloads;

public sealed record UpdateProductPricePayload(
    ProductDto? Product,
    IReadOnlyList<PayloadError> Errors)
{
    public static UpdateProductPricePayload FromResult(ErrorOr<ProductDto> result)
    {
        return result.IsError
            ? new UpdateProductPricePayload(null, result.Errors.Select(GraphQLErrorFactory.ToPayloadError).ToList())
            : new UpdateProductPricePayload(result.Value, []);
    }
}
