using GreenDonut;
using HotChocolateDddCqrsTemplate.Application.Catalog;
using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;

namespace HotChocolateDddCqrsTemplate.Api.GraphQL.DataLoaders;

public sealed class CategoryByIdDataLoader(
    IBatchScheduler batchScheduler,
    ICategoryRepository categoryRepository)
    : BatchDataLoader<Guid, CategoryDto?>(batchScheduler, new DataLoaderOptions())
{
    protected override async Task<IReadOnlyDictionary<Guid, CategoryDto?>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetByIdsAsync(keys.Distinct().ToArray(), cancellationToken);

        return keys.Distinct().ToDictionary(
            key => key,
            key => categories.TryGetValue(key, out var category) ? category.ToDto() : null);
    }
}
