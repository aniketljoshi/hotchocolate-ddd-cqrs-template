using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Application.Common.Interfaces;
using HotChocolateDddCqrsTemplate.Application.Common.Models;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Queries.ListProducts;

public sealed record ListProductsQuery(int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResult<ProductDto>>;
