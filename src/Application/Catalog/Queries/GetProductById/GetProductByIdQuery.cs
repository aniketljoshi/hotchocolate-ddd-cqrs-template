using ErrorOr;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Application.Common.Interfaces;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid ProductId) : IQuery<ErrorOr<ProductDto>>;
