namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Inputs;

public sealed record UpdateProductPriceInput(
    Guid ProductId,
    decimal Price,
    string Currency);
