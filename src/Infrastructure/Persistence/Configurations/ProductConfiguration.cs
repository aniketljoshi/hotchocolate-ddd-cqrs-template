using HotChocolateDddCqrsTemplate.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.CategoryId)
            .IsRequired();

        builder.Property(product => product.IsArchived)
            .IsRequired();

        builder.OwnsOne(product => product.Price, ownedBuilder =>
        {
            ownedBuilder.Property(price => price.Amount)
                .HasColumnName("price_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            ownedBuilder.Property(price => price.Currency)
                .HasColumnName("price_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(product => product.CostPrice, ownedBuilder =>
        {
            ownedBuilder.Property(price => price.Amount)
                .HasColumnName("cost_price_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            ownedBuilder.Property(price => price.Currency)
                .HasColumnName("cost_price_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(product => product.Sku, ownedBuilder =>
        {
            ownedBuilder.Property(sku => sku.Value)
                .HasColumnName("sku")
                .HasMaxLength(32)
                .IsRequired();

            ownedBuilder.HasIndex(sku => sku.Value)
                .IsUnique();
        });

        builder.HasIndex(product => product.CategoryId);
    }
}
