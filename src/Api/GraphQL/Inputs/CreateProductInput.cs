namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Inputs;

public sealed record CreateProductInput(
    string Name,
    decimal Price,
    string Currency,
    string Sku,
    Guid CategoryId);
