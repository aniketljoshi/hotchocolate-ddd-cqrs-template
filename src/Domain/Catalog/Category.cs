using HotChocolateDddCqrsTemplate.Domain.Common;

namespace HotChocolateDddCqrsTemplate.Domain.Catalog;

public sealed class Category : Entity
{
    private Category()
    {
        Name = string.Empty;
        Slug = string.Empty;
    }

    private Category(Guid id, string name, string slug)
        : base(id)
    {
        Name = name;
        Slug = slug;
    }

    public string Name { get; private set; }

    public string Slug { get; private set; }

    public static Category Create(string name, string slug, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Category slug is required.", nameof(slug));
        }

        return new Category(
            id ?? Guid.NewGuid(),
            name.Trim(),
            slug.Trim().ToLowerInvariant());
    }
}
