using HotChocolateDddCqrsTemplate.Application.Catalog.Queries.GetProductById;
using HotChocolateDddCqrsTemplate.Domain.Catalog;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;
using HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;

namespace HotChocolateDddCqrsTemplate.Application.Tests.Catalog;

public sealed class GetProductByIdQueryTests
{
    [Fact]
    public async Task Handle_WhenProductExists_ShouldReturnDto()
    {
        var product = Product.Create(
            "Found Product",
            Money.Create(99m, "USD"),
            Sku.Create("FND-001"),
            Guid.NewGuid());

        var handler = new GetProductByIdQueryHandler(new InMemoryProductRepository(product));

        var result = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        Assert.False(result.IsError);
        Assert.Equal(product.Id, result.Value.Id);
        Assert.Equal("Found Product", result.Value.Name);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ShouldReturnNotFound()
    {
        var handler = new GetProductByIdQueryHandler(new InMemoryProductRepository(null));

        var result = await handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal("Catalog.Product.NotFound", result.FirstError.Code);
    }

    private sealed class InMemoryProductRepository(Product? product) : IProductRepository
    {
        public Task AddAsync(Product newProduct, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(product?.Id == id ? product : null);
        }

        public Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Product>>(product is null ? [] : [product]);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(product is null ? 0 : 1);
        }

        public Task<bool> ExistsBySkuAsync(Sku sku, CancellationToken cancellationToken)
        {
            return Task.FromResult(product?.Sku.Value == sku.Value);
        }

        public Task<IReadOnlyDictionary<Guid, Product>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken)
        {
            IReadOnlyDictionary<Guid, Product> result = product is not null && ids.Contains(product.Id)
                ? new Dictionary<Guid, Product> { [product.Id] = product }
                : new Dictionary<Guid, Product>();

            return Task.FromResult(result);
        }
    }
}
