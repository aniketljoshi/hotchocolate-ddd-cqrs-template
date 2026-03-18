using HotChocolateDddCqrsTemplate.Domain.Catalog;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;
using HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;
using HotChocolateDddCqrsTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Repositories;

public sealed class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Products
            .SingleOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return dbContext.Products.CountAsync(cancellationToken);
    }

    public Task<bool> ExistsBySkuAsync(Sku sku, CancellationToken cancellationToken)
    {
        return dbContext.Products
            .AnyAsync(product => product.Sku.Value == sku.Value, cancellationToken);
    }
}
