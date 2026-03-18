using ErrorOr;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Application.Common.Interfaces;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Commands.UpdateProductPrice;

public sealed record UpdateProductPriceCommand(
    Guid ProductId,
    decimal Price,
    string Currency) : ICommand<ErrorOr<ProductDto>>;
