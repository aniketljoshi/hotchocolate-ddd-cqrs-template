using HotChocolate;
using HotChocolate.Types;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Errors;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Application.Catalog.Queries.GetProductById;
using HotChocolateDddCqrsTemplate.Application.Catalog.Queries.ListCategories;
using HotChocolateDddCqrsTemplate.Application.Catalog.Queries.ListProducts;
using HotChocolateDddCqrsTemplate.Application.Common.Models;
using MediatR;

namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class ProductQueries
{
    public async Task<ProductDto?> ProductByIdAsync(
        Guid id,
        [Service] ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);

        if (result.IsError)
        {
            throw GraphQLErrorFactory.ToGraphQLException(result.Errors);
        }

        return result.Value;
    }

    public Task<PagedResult<ProductDto>> ProductsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        [Service] ISender sender = default!,
        CancellationToken cancellationToken = default)
    {
        return sender.Send(new ListProductsQuery(pageNumber, pageSize), cancellationToken);
    }

    public Task<IReadOnlyList<CategoryDto>> CategoriesAsync(
        [Service] ISender sender,
        CancellationToken cancellationToken)
    {
        return sender.Send(new ListCategoriesQuery(), cancellationToken);
    }
}
