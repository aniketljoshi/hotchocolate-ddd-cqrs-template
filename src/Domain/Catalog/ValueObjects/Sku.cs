using System.Text.RegularExpressions;
using HotChocolateDddCqrsTemplate.Domain.Common;

namespace HotChocolateDddCqrsTemplate.Domain.Catalog.ValueObjects;

public sealed partial class Sku : ValueObject
{
    private Sku()
    {
        Value = string.Empty;
    }

    private Sku(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static Sku Create(string value)
    {
        var normalized = value.Trim().ToUpperInvariant();

        if (!SkuPattern().IsMatch(normalized))
        {
            throw new ArgumentException("SKU must be 3-32 characters using letters, numbers, or dashes.", nameof(value));
        }

        return new Sku(normalized);
    }

    public bool IsValid()
    {
        return SkuPattern().IsMatch(Value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex("^[A-Z0-9-]{3,32}$", RegexOptions.Compiled)]
    private static partial Regex SkuPattern();
}
