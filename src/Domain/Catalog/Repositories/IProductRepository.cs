using HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;

namespace HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);

    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<bool> ExistsBySkuAsync(Sku sku, CancellationToken cancellationToken);
}
