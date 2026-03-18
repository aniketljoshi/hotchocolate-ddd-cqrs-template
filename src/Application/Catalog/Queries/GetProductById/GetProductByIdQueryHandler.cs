using ErrorOr;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;
using MediatR;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductByIdQuery, ErrorOr<ProductDto>>
{
    public async Task<ErrorOr<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        return product is null
            ? CatalogErrors.ProductNotFound(request.ProductId)
            : product.ToDto();
    }
}
