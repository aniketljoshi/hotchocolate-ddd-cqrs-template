using HotChocolateDddCqrsTemplate.Domain.Catalog;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Events;
using HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;

namespace HotChocolateDddCqrsTemplate.Domain.Tests.Catalog;

public sealed class ProductTests
{
    [Fact]
    public void Create_ShouldRaiseProductCreatedDomainEvent()
    {
        var categoryId = Guid.NewGuid();

        var product = Product.Create(
            "GraphQL in Action",
            Money.Create(129.99m, "USD"),
            Sku.Create("BOOK-001"),
            categoryId);

        var domainEvent = Assert.Single(product.DomainEvents);
        var createdEvent = Assert.IsType<ProductCreatedDomainEvent>(domainEvent);

        Assert.Equal(product.Id, createdEvent.ProductId);
        Assert.Equal("GraphQL in Action", createdEvent.Name);
        Assert.Equal(129.99m, createdEvent.Price);
        Assert.Equal("USD", createdEvent.Currency);
    }

    [Fact]
    public void UpdatePrice_ShouldRaisePriceChangedEvent()
    {
        var product = Product.Create(
            "Starter Kit",
            Money.Create(100m, "USD"),
            Sku.Create("KIT-001"),
            Guid.NewGuid());

        product.ClearDomainEvents();

        product.UpdatePrice(Money.Create(125m, "USD"));

        var domainEvent = Assert.Single(product.DomainEvents);
        var priceChangedEvent = Assert.IsType<ProductPriceChangedDomainEvent>(domainEvent);

        Assert.Equal(100m, priceChangedEvent.OldPrice);
        Assert.Equal(125m, priceChangedEvent.NewPrice);
        Assert.Equal("USD", priceChangedEvent.Currency);
    }

    [Fact]
    public void UpdatePrice_WhenArchived_ShouldThrow()
    {
        var product = Product.Create(
            "Archived Product",
            Money.Create(10m, "USD"),
            Sku.Create("ARC-001"),
            Guid.NewGuid());

        product.Archive();

        Assert.Throws<InvalidOperationException>(() => product.UpdatePrice(Money.Create(12m, "USD")));
    }
}
