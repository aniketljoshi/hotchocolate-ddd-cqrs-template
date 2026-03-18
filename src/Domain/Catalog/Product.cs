using HotChocolateDddCqrsTemplate.Domain.Catalog.Events;
using HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;
using HotChocolateDddCqrsTemplate.Domain.Common;

namespace HotChocolateDddCqrsTemplate.Domain.Catalog;

public sealed class Product : AggregateRoot
{
    private Product()
    {
        Name = string.Empty;
        Price = default!;
        CostPrice = default!;
        Sku = default!;
    }

    private Product(Guid id, string name, Money price, Money costPrice, Sku sku, Guid categoryId)
        : base(id)
    {
        Name = name;
        Price = price;
        CostPrice = costPrice;
        Sku = sku;
        CategoryId = categoryId;
    }

    public string Name { get; private set; }

    public Money Price { get; private set; }

    /// <summary>
    /// Internal cost price — visible only to users with the <c>inventory-manager</c> policy.
    /// Demonstrates field-level authorization in HotChocolate.
    /// </summary>
    public Money CostPrice { get; private set; }

    public Sku Sku { get; private set; }

    public Guid CategoryId { get; private set; }

    public bool IsArchived { get; private set; }

    public static Product Create(string name, Money price, Sku sku, Guid categoryId, Guid? id = null, Money? costPrice = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.", nameof(name));
        }

        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category is required.", nameof(categoryId));
        }

        var actualCostPrice = costPrice ?? Money.Create(Math.Round(price.Amount * 0.6m, 2), price.Currency);
        var product = new Product(id ?? Guid.NewGuid(), name.Trim(), price, actualCostPrice, sku, categoryId);

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(
            product.Id,
            product.Name,
            product.Price.Amount,
            product.Price.Currency,
            DateTime.UtcNow));

        return product;
    }

    public void UpdatePrice(Money newPrice)
    {
        if (IsArchived)
        {
            throw new InvalidOperationException("Archived products cannot be updated.");
        }

        if (Price == newPrice)
        {
            return;
        }

        var previousPrice = Price;
        Price = newPrice;

        RaiseDomainEvent(new ProductPriceChangedDomainEvent(
            Id,
            previousPrice.Amount,
            newPrice.Amount,
            newPrice.Currency,
            DateTime.UtcNow));
    }

    public void Archive()
    {
        IsArchived = true;
    }
}
