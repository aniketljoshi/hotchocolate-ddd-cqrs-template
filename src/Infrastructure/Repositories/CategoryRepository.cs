using HotChocolateDddCqrsTemplate.Domain.Catalog;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;
using HotChocolateDddCqrsTemplate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Repositories;

public sealed class CategoryRepository(ApplicationDbContext dbContext) : ICategoryRepository
{
    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        await dbContext.Categories.AddAsync(category, cancellationToken);
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Categories
            .AsNoTracking()
            .SingleOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, Category>> GetByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        var categories = await dbContext.Categories
            .AsNoTracking()
            .Where(category => ids.Contains(category.Id))
            .ToListAsync(cancellationToken);

        return categories.ToDictionary(category => category.Id);
    }

    public async Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Categories
            .AnyAsync(category => category.Id == id, cancellationToken);
    }
}
