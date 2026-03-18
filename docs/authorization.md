# Field-Level Authorization

This template includes a working example of **field-level authorization** using HotChocolate's built-in authorization middleware.

## The Pattern

Not every field in a GraphQL type should be visible to every user. Cost prices, internal notes, and audit metadata are examples of fields that should be restricted.

HotChocolate supports this via the `Authorize` directive on individual fields:

```csharp
// In ProductType.cs
descriptor.Field(product => product.CostPrice)
    .Authorize(CatalogAuthorizationPolicies.InventoryManager);
```

## How It Works

1. **Policy definition** — `CatalogAuthorizationPolicies.cs` defines named policy constants.
2. **Policy registration** — `Program.cs` registers ASP.NET Core authorization policies.
3. **Field protection** — `ProductType` applies `Authorize` to the `costPrice` field.
4. **Runtime behavior** — Unauthorized users receive `null` for protected fields with an error extension indicating authorization failure.

## Policy Setup

```csharp
// Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(CatalogAuthorizationPolicies.InventoryManager, policy =>
        policy.RequireClaim("role", "inventory-manager"));
});
```

In production, replace `RequireClaim` with your identity provider's mechanism:

| Provider | Approach |
|---|---|
| Keycloak / OIDC | `policy.RequireRole("inventory-manager")` or custom claim |
| Azure AD | `policy.RequireClaim("groups", "<group-id>")` |
| Auth0 | `policy.RequireClaim("permissions", "read:cost-prices")` |
| Custom JWT | `policy.RequireClaim("role", "inventory-manager")` |

## Testing Authorization

### Authorized request (returns cost price)

```graphql
query {
  productById(id: "4f2bde1d-0f65-488d-bca5-6efc5dd55b0d") {
    name
    price
    costPrice          # ← returns value
    costPriceCurrency  # ← returns value
  }
}
```

### Unauthorized request (field returns null with error)

```graphql
# Without inventory-manager role:
query {
  productById(id: "4f2bde1d-0f65-488d-bca5-6efc5dd55b0d") {
    name
    price
    costPrice          # ← null
    costPriceCurrency  # ← null
  }
}
```

Response includes:

```json
{
  "errors": [
    {
      "message": "The current user is not authorized to access this resource.",
      "path": ["productById", "costPrice"],
      "extensions": {
        "code": "AUTH_NOT_AUTHORIZED"
      }
    }
  ]
}
```

## Architecture Decision

Field-level authorization lives in the **Api layer** (GraphQL type configuration), not in the domain or application layer. This follows the principle that authorization is a delivery concern — the domain enforces business rules, the API enforces access rules.

See also: [ADR-003 — Thin GraphQL Resolvers](decisions/ADR-003-thin-graphql-resolvers.md)
