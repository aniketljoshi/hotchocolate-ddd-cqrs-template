namespace HotChocolateDddCqrsTemplate.Api.GraphQL.Authorization;

/// <summary>
/// Defines authorization policy names used by GraphQL field-level auth.
/// Wire up actual policy requirements in <see cref="DependencyInjection"/>.
/// </summary>
public static class CatalogAuthorizationPolicies
{
    /// <summary>
    /// Required to view cost-sensitive fields such as <c>Product.costPrice</c>.
    /// Map this to a real claim, role, or scope in your identity provider.
    /// </summary>
    public const string InventoryManager = "inventory-manager";
}
