using ErrorOr;

namespace HotChocolateDddCqrsTemplate.Application.Catalog;

public static class CatalogErrors
{
    public static Error ProductNotFound(Guid productId) =>
        Error.NotFound("Catalog.Product.NotFound", $"Product '{productId}' was not found.");

    public static Error CategoryNotFound(Guid categoryId) =>
        Error.NotFound("Catalog.Category.NotFound", $"Category '{categoryId}' was not found.");

    public static Error DuplicateSku(string sku) =>
        Error.Conflict("Catalog.Product.DuplicateSku", $"A product with SKU '{sku}' already exists.");

    public static Error ProductArchived(Guid productId) =>
        Error.Failure("Catalog.Product.Archived", $"Product '{productId}' is archived and cannot be changed.");
}
