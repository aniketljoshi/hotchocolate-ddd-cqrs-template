using HotChocolateDddCqrsTemplate.Application.Catalog.DTOs;
using HotChocolateDddCqrsTemplate.Domain.Catalog;

namespace HotChocolateDddCqrsTemplate.Application.Catalog;

public static class CatalogMappings
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Price.Amount,
            product.Price.Currency,
            product.CostPrice.Amount,
            product.CostPrice.Currency,
            product.Sku.Value,
            product.CategoryId,
            product.IsArchived);
    }

    public static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.Slug);
    }
}
