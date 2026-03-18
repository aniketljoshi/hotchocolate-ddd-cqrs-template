using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Application.Common.Models;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;
using MediatR;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Queries.ListProducts;

public sealed class ListProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<ListProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var products = await productRepository.ListAsync(pageNumber, pageSize, cancellationToken);
        var totalCount = await productRepository.CountAsync(cancellationToken);

        return new PagedResult<ProductDto>(
            products.Select(product => product.ToDto()).ToList(),
            pageNumber,
            pageSize,
            totalCount);
    }
}
