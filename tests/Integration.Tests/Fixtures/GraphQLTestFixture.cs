using HotChocolateDddCqrsTemplate.Domain.Catalog;
using HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;
using HotChocolateDddCqrsTemplate.Infrastructure.Observability;
using HotChocolateDddCqrsTemplate.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace HotChocolateDddCqrsTemplate.Integration.Tests.Fixtures;

public sealed class GraphQLTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("hotchocolate_template_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private TestWebApplicationFactory? _factory;

    public HttpClient Client { get; private set; } = default!;

    public IServiceProvider Services => _factory!.Services;

    public CatalogQueryCounter QueryCounter => Services.GetRequiredService<CatalogQueryCounter>();

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        Environment.SetEnvironmentVariable("ConnectionStrings__CatalogDb", _postgresContainer.GetConnectionString());
        _factory = new TestWebApplicationFactory(_postgresContainer.GetConnectionString());
        Client = _factory.CreateClient();

        await ResetAsync();
    }

    public async Task DisposeAsync()
    {
        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }

        Environment.SetEnvironmentVariable("ConnectionStrings__CatalogDb", null);
        await _postgresContainer.DisposeAsync();
    }

    public async Task ResetAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        QueryCounter.Reset();

        var categories = new[]
        {
            Category.Create("Books", "books", Guid.Parse("820b63b4-ec53-4f06-9871-57d9bf14bb51")),
            Category.Create("Courses", "courses", Guid.Parse("d8ee54d4-cce8-4948-a0ee-ebea85a34895"))
        };

        await dbContext.Categories.AddRangeAsync(categories);

        var products = Enumerable.Range(1, 10)
            .Select(index => Product.Create(
                $"Product {index}",
                Money.Create(50 + index, "USD"),
                Sku.Create($"SKU-{index:000}"),
                index % 2 == 0 ? categories[0].Id : categories[1].Id))
            .ToList();

        foreach (var product in products)
        {
            product.ClearDomainEvents();
        }

        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();
    }

    public async Task<Guid> GetFirstCategoryIdAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Categories.Select(category => category.Id).FirstAsync();
    }

    private sealed class TestWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("IntegrationTest");

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:CatalogDb"] = connectionString
                });
            });
        }
    }
}
