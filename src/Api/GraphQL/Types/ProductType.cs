using HotChocolate.Authorization;
using HotChocolate.Types;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Authorization;
using HotChocolateDddCqrsTemplate.Api.GraphQL.DataLoaders;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;

namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Types;

public sealed class ProductType : ObjectType<ProductDto>
{
    protected override void Configure(IObjectTypeDescriptor<ProductDto> descriptor)
    {
        descriptor.Field(product => product.CategoryId).Ignore();

        descriptor.Field("category")
            .Type<CategoryType>()
            .Resolve(async context =>
            {
                var product = context.Parent<ProductDto>();
                var dataLoader = context.DataLoader<CategoryByIdDataLoader>();
                return await dataLoader.LoadAsync(product.CategoryId, context.RequestAborted);
            });

        // Field-level authorization: costPrice is only visible to inventory managers.
        // Unauthorized users receive null with an AUTH_NOT_AUTHORIZED error extension.
        descriptor.Field(product => product.CostPrice)
            .Authorize(CatalogAuthorizationPolicies.InventoryManager);

        descriptor.Field(product => product.CostPriceCurrency)
            .Authorize(CatalogAuthorizationPolicies.InventoryManager);
    }
}
