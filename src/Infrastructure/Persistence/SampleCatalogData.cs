using HotChocolateDddCqrsTemplate.Domain.Catalog;
using HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Persistence;

public static class SampleCatalogData
{
    public static readonly Guid BooksCategoryId = Guid.Parse("820b63b4-ec53-4f06-9871-57d9bf14bb51");
    public static readonly Guid CoursesCategoryId = Guid.Parse("d8ee54d4-cce8-4948-a0ee-ebea85a34895");
    public static readonly Guid DomainDrivenDesignBookId = Guid.Parse("4f2bde1d-0f65-488d-bca5-6efc5dd55b0d");
    public static readonly Guid HotChocolateCourseId = Guid.Parse("c8d8a522-67f5-4ee6-8bf0-05a6ee1c7f21");
    public static readonly Guid CqrsStarterKitId = Guid.Parse("16a5d6e8-d711-4722-a968-c328a8f1fa84");
    public static readonly Guid CleanArchitectureNotebookId = Guid.Parse("cf8c6717-01ee-4df6-a1e4-c1f41063f636");

    public static async Task SeedAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Categories.AnyAsync(cancellationToken) || await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var categories = new[]
        {
            Category.Create("Books", "books", BooksCategoryId),
            Category.Create("Courses", "courses", CoursesCategoryId)
        };

        var products = new[]
        {
            Product.Create(
                "Domain-Driven Design Distilled",
                Money.Create(44.0m, "USD"),
                Sku.Create("BOOK-001"),
                BooksCategoryId,
                DomainDrivenDesignBookId),
            Product.Create(
                "HotChocolate in Practice",
                Money.Create(129.0m, "USD"),
                Sku.Create("COURSE-001"),
                CoursesCategoryId,
                HotChocolateCourseId),
            Product.Create(
                "CQRS Starter Kit",
                Money.Create(89.0m, "USD"),
                Sku.Create("COURSE-002"),
                CoursesCategoryId,
                CqrsStarterKitId),
            Product.Create(
                "Clean Architecture Notebook",
                Money.Create(29.0m, "USD"),
                Sku.Create("BOOK-002"),
                BooksCategoryId,
                CleanArchitectureNotebookId)
        };

        foreach (var product in products)
        {
            product.ClearDomainEvents();
        }

        await dbContext.Categories.AddRangeAsync(categories, cancellationToken);
        await dbContext.Products.AddRangeAsync(products, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
