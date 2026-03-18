using HotChocolate;
using HotChocolate.Types;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Inputs;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Payloads;
using HotChocolateDddCqrsTemplate.Application.Catalog.Commands.CreateProduct;
using HotChocolateDddCqrsTemplate.Application.Catalog.Commands.UpdateProductPrice;
using MediatR;

namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public sealed class ProductMutations
{
    public async Task<CreateProductPayload> CreateProductAsync(
        CreateProductInput input,
        [Service] ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateProductCommand(input.Name, input.Price, input.Currency, input.Sku, input.CategoryId),
            cancellationToken);

        return CreateProductPayload.FromResult(result);
    }

    public async Task<UpdateProductPricePayload> UpdateProductPriceAsync(
        UpdateProductPriceInput input,
        [Service] ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateProductPriceCommand(input.ProductId, input.Price, input.Currency),
            cancellationToken);

        return UpdateProductPricePayload.FromResult(result);
    }
}
