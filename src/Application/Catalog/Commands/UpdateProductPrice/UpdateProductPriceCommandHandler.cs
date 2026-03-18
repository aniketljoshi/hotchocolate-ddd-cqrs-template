using ErrorOr;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;
using HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;
using MediatR;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Commands.UpdateProductPrice;

public sealed class UpdateProductPriceCommandHandler(IProductRepository productRepository)
    : IRequestHandler<UpdateProductPriceCommand, ErrorOr<ProductDto>>
{
    public async Task<ErrorOr<ProductDto>> Handle(UpdateProductPriceCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return CatalogErrors.ProductNotFound(request.ProductId);
        }

        try
        {
            product.UpdatePrice(Money.Create(request.Price, request.Currency));
        }
        catch (InvalidOperationException)
        {
            return CatalogErrors.ProductArchived(request.ProductId);
        }

        return product.ToDto();
    }
}
