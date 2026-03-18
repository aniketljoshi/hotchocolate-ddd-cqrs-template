using GreenDonut;
using HotChocolateDddCqrsTemplate.Application.Catalog;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;

namespace HotChocolateDddCqrsTemplate.Api.GraphQL.DataLoaders;

public sealed class ProductByIdDataLoader(
    IBatchScheduler batchScheduler,
    IProductRepository productRepository)
    : BatchDataLoader<Guid, ProductDto?>(batchScheduler, new DataLoaderOptions())
{
    protected override async Task<IReadOnlyDictionary<Guid, ProductDto?>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        var products = await productRepository.GetByIdsAsync(keys.Distinct().ToArray(), cancellationToken);

        return keys.Distinct().ToDictionary(
            key => key,
            key => products.TryGetValue(key, out var product) ? product.ToDto() : null);
    }
}
