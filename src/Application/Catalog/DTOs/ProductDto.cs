namespace HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;

public sealed record ProductDto(
    Guid Id,
    string Name,
    decimal Price,
    string Currency,
    string Sku,
    Guid CategoryId,
    bool IsArchived);
