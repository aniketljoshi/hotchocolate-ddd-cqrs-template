using HotChocolate.Types;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Application.Common.Models;

namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Types;

public sealed class PagedProductResultType : ObjectType<PagedResult<ProductDto>>
{
    protected override void Configure(IObjectTypeDescriptor<PagedResult<ProductDto>> descriptor)
    {
        descriptor.Field(result => result.Items)
            .Type<NonNullType<ListType<NonNullType<ProductType>>>>();
    }
}
