using ErrorOr;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Application.Common.Interfaces;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    decimal Price,
    string Currency,
    string Sku,
    Guid CategoryId) : ICommand<ErrorOr<ProductDto>>;
