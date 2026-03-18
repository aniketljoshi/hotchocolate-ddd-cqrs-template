# ADR-003: Thin GraphQL Resolvers

## Status

Accepted

## Context

In many HotChocolate samples, GraphQL resolvers contain business logic: validation, database queries, conditional branching, and error handling all live inside the resolver method. This creates several problems:

1. **Untestable without GraphQL** — testing a resolver requires spinning up the full HotChocolate pipeline.
2. **Duplicate logic** — if the same operation is needed from a REST endpoint or background job, the logic must be duplicated.
3. **Layer violation** — the API layer becomes the application layer.

## Decision

GraphQL resolvers are **thin delivery adapters**. A resolver may only:

1. Call `ISender.Send()` with a MediatR command or query.
2. Map the result to a GraphQL payload type.
3. Apply GraphQL-specific concerns (DataLoader batching, authorization attributes).

```csharp
[MutationType]
public sealed class ProductMutations
{
    public async Task<CreateProductPayload> CreateProductAsync(
        CreateProductInput input,
        [Service] ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateProductCommand(input.Name, input.Price, input.Currency, input.Sku, input.CategoryId),
            cancellationToken);

        return CreateProductPayload.FromResult(result);
    }
}
```

All business logic, validation, persistence, and domain event handling live in the **Application layer** handlers.

## Consequences

### Positive

- **Testable in isolation** — command/query handlers can be unit-tested with mock repositories, no GraphQL runtime needed.
- **Reusable** — the same handler serves GraphQL, REST, gRPC, or background jobs.
- **Clear boundaries** — the Api layer is a mapping layer; the Application layer owns behavior.
- **Consistent error handling** — `ErrorOr<T>` flows through the MediatR pipeline and is mapped to GraphQL errors at the boundary.

### Negative

- **Indirection** — an extra hop through MediatR for every request. Acceptable for the consistency it provides.
- **Boilerplate** — each operation needs an Input, Command/Query, Handler, and Payload. Tooling (file templates, snippets) mitigates this.

## Alternatives Considered

- **Fat resolvers with injected services** — rejected because it couples business logic to the GraphQL layer and prevents reuse.
- **Service layer without MediatR** — viable but loses the pipeline behavior model (logging, validation, domain event dispatch) that MediatR provides for free.
