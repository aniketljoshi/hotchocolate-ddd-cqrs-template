using System.Diagnostics;
using HotChocolate;
using HotChocolate.Types;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Inputs;
using HotChocolateDddCqrsTemplate.Api.GraphQL.Payloads;
using HotChocolateDddCqrsTemplate.Application.Catalog.Commands.CreateProduct;
using HotChocolateDddCqrsTemplate.Application.Catalog.Commands.UpdateProductPrice;
using HotChocolateDddCqrsTemplate.Application.Common.Observability;
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
        using var activity = TemplateTelemetry.ApiActivitySource.StartActivity(
            "graphql.mutation.createProduct",
            ActivityKind.Server);

        activity?.SetTag("graphql.sku", input.Sku);
        activity?.SetTag("graphql.category_id", input.CategoryId);

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
        using var activity = TemplateTelemetry.ApiActivitySource.StartActivity(
            "graphql.mutation.updateProductPrice",
            ActivityKind.Server);

        activity?.SetTag("graphql.product_id", input.ProductId);
        activity?.SetTag("graphql.price", input.Price);
        activity?.SetTag("graphql.currency", input.Currency);

        var result = await sender.Send(
            new UpdateProductPriceCommand(input.ProductId, input.Price, input.Currency),
            cancellationToken);

        return UpdateProductPricePayload.FromResult(result);
    }
}
