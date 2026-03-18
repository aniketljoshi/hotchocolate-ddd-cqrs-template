using ErrorOr;
using HotChocolateDddCqrsTemplate.Application.Catalog.Commands.CreateProduct;
using HotChocolateDddCqrsTemplate.Domain.Catalog;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Repositories;
using HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;

namespace HotChocolateDddCqrsTemplate.Application.Tests.Catalog;

public sealed class CreateProductCommandTests
{
    [Fact]
    public async Task Handle_WhenSkuAlreadyExists_ShouldReturnConflict()
    {
        var productRepository = new InMemoryProductRepository
        {
            ExistingSku = Sku.Create("DUP-001")
        };

        var categoryRepository = new InMemoryCategoryRepository();
        categoryRepository.AddSeed(Category.Create("Books", "books", Guid.Parse("4a7f9166-b68c-44df-96b9-af544f1828eb")));

        var handler = new CreateProductCommandHandler(productRepository, categoryRepository);

        var result = await handler.Handle(
            new CreateProductCommand("GraphQL Book", 49.99m, "USD", "DUP-001", Guid.Parse("4a7f9166-b68c-44df-96b9-af544f1828eb")),
            CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal("Catalog.Product.DuplicateSku", result.FirstError.Code);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCreateProduct()
    {
        var productRepository = new InMemoryProductRepository();
        var categoryRepository = new InMemoryCategoryRepository();
        var category = Category.Create("Software", "software");
        categoryRepository.AddSeed(category);

        var handler = new CreateProductCommandHandler(productRepository, categoryRepository);

        var result = await handler.Handle(
            new CreateProductCommand("HotChocolate Template", 199m, "USD", "TPL-001", category.Id),
            CancellationToken.None);

        Assert.False(result.IsError);
        Assert.NotNull(productRepository.AddedProduct);
        Assert.Equal("HotChocolate Template", result.Value.Name);
        Assert.Equal(category.Id, result.Value.CategoryId);
    }

    private sealed class InMemoryProductRepository : IProductRepository
    {
        public Product? AddedProduct { get; private set; }

        public Sku? ExistingSku { get; init; }

        public Task AddAsync(Product product, CancellationToken cancellationToken)
        {
            AddedProduct = product;
            return Task.CompletedTask;
        }

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult<Product?>(AddedProduct);
        }

        public Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            IReadOnlyList<Product> products = AddedProduct is null ? [] : [AddedProduct];
            return Task.FromResult(products);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(AddedProduct is null ? 0 : 1);
        }

        public Task<bool> ExistsBySkuAsync(Sku sku, CancellationToken cancellationToken)
        {
            return Task.FromResult(ExistingSku?.Value == sku.Value);
        }

        public Task<IReadOnlyDictionary<Guid, Product>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken)
        {
            IReadOnlyDictionary<Guid, Product> result = AddedProduct is not null && ids.Contains(AddedProduct.Id)
                ? new Dictionary<Guid, Product> { [AddedProduct.Id] = AddedProduct }
                : new Dictionary<Guid, Product>();

            return Task.FromResult(result);
        }
    }

    private sealed class InMemoryCategoryRepository : ICategoryRepository
    {
        private readonly Dictionary<Guid, Category> _categories = [];

        public Task AddAsync(Category category, CancellationToken cancellationToken)
        {
            _categories[category.Id] = category;
            return Task.CompletedTask;
        }

        public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _categories.TryGetValue(id, out var category);
            return Task.FromResult(category);
        }

        public Task<IReadOnlyDictionary<Guid, Category>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
        {
            IReadOnlyDictionary<Guid, Category> categories = _categories
                .Where(pair => ids.Contains(pair.Key))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            return Task.FromResult(categories);
        }

        public Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Category>>(_categories.Values.ToList());
        }

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_categories.ContainsKey(id));
        }

        public void AddSeed(Category category)
        {
            _categories[category.Id] = category;
        }
    }
}
