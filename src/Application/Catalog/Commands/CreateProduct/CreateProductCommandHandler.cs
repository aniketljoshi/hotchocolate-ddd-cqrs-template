using ErrorOr;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Domain.Catalog;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;
using HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;
using MediatR;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository)
    : IRequestHandler<CreateProductCommand, ErrorOr<ProductDto>>
{
    public async Task<ErrorOr<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var sku = Sku.Create(request.Sku);

        if (await productRepository.ExistsBySkuAsync(sku, cancellationToken))
        {
            return CatalogErrors.DuplicateSku(request.Sku);
        }

        if (!await categoryRepository.ExistsAsync(request.CategoryId, cancellationToken))
        {
            return CatalogErrors.CategoryNotFound(request.CategoryId);
        }

        var product = Product.Create(
            request.Name,
            Money.Create(request.Price, request.Currency),
            sku,
            request.CategoryId);

        await productRepository.AddAsync(product, cancellationToken);

        return product.ToDto();
    }
}
