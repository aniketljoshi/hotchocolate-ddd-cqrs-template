namespace HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;

public sealed record CategoryDto(
    Guid Id,
    string Name,
    string Slug);
