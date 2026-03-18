# hotchocolate-ddd-cqrs-template

**Enterprise GraphQL Template for .NET**
HotChocolate 15 · DDD · CQRS · MediatR · Outbox Pattern · .NET 10 LTS

[![.NET](https://img.shields.io/badge/.NET-10.0_LTS-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![HotChocolate](https://img.shields.io/badge/HotChocolate-v15-E10098)](https://chillicream.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Stars](https://img.shields.io/github/stars/aniketljoshi/hotchocolate-ddd-cqrs-template?style=social)](https://github.com/aniketljoshi/hotchocolate-ddd-cqrs-template)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

---

A production-leaning .NET 10 template for teams building GraphQL backends with HotChocolate, clean architecture, and a reliable domain event pipeline.

> GraphQL is the delivery layer.  
> Business logic stays in the application and domain layers.

---

## Why This Exists

Most .NET GraphQL samples are useful for learning but stop short of production architecture:

| Problem | Most samples | This template |
|---|---|---|
| HotChocolate version | v13 (2023) | ✅ v15 (2025) |
| .NET version | 6 or 7 | ✅ 10 LTS |
| Resolver pattern | Business logic in resolvers | ✅ Thin resolvers → MediatR |
| Domain events | Implement `INotification` directly | ✅ Pure domain, mapped in Application |
| Event reliability | Direct publish after SaveChanges | ✅ Outbox Pattern — crash-safe |
| Architecture tests | Missing | ✅ NetArchTest enforces layers |
| DataLoader | Missing or incomplete | ✅ Shown with N+1 proof |
| Field authorization | Missing | ✅ Field-level example included |

---

## Architecture

```
GraphQL Request
      │
      ▼
HotChocolate v15 Resolver
(thin — sends MediatR command or query, nothing else)
      │
      ▼
MediatR Pipeline
  ├── LoggingBehavior
  ├── ValidationBehavior        (FluentValidation)
  └── DomainEventDispatchBehavior  ← fires AFTER SaveChanges
      │
      ▼
Command / Query Handler
      │
      ▼
Domain Aggregate Root
(all business logic lives here)
      │  raises
      ▼
IDomainEvent
(pure C# — zero framework dependencies in Domain layer)
      │  saved atomically with aggregate
      ▼
Outbox Table
      │
      ▼
Background Processor → INotificationHandler
```

See [docs/architecture.md](docs/architecture.md) for full diagrams.

---

## The Outbox Pattern — Why It Matters

Most templates do this:

```csharp
// ❌ SILENT DATA LOSS
// If the app crashes between these two lines, the event is gone forever.
// No exception. No log entry. Silent.
await _dbContext.SaveChangesAsync();
await _messageBus.PublishAsync(new ProductCreatedEvent(product.Id));
```

This template does this:

```csharp
// ✅ CRASH-SAFE
// OutboxMessage saves in the SAME database transaction as the aggregate.
// If the app crashes, the background processor picks it up on restart.
// Nothing is lost.
await _dbContext.SaveChangesAsync(); // saves Product + OutboxMessage atomically

// OutboxProcessor (background service) reads pending messages and dispatches them.
// Guaranteed at-least-once delivery.
```

See [docs/outbox-pattern.md](docs/outbox-pattern.md) for the full implementation walkthrough.

---

## Domain Events Without MediatR

Most templates couple domain events to MediatR:

```csharp
// ❌ Domain layer now depends on an external framework
public class ProductCreatedEvent : INotification { ... }
```

This template keeps the domain pure:

```csharp
// ✅ Domain layer has ZERO external dependencies
public class ProductCreatedEvent : IDomainEvent { ... }

// Application layer maps to INotification before dispatch
internal sealed class ProductCreatedNotification : INotification
{
    public ProductCreatedNotification(ProductCreatedEvent domainEvent) { ... }
}
```

Your domain is testable without any framework installed.
See [docs/decisions/ADR-001-domain-events-not-mediatr.md](docs/decisions/ADR-001-domain-events-not-mediatr.md).

---

## Thin Resolver Pattern (HC v15)

```csharp
// This is ALL the resolver does.
[QueryType]
public sealed class ProductQueries
{
    public async Task<ProductDto> GetProductByIdAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
        => await sender.Send(new GetProductByIdQuery(id), cancellationToken);
}
```

```csharp
// Business logic lives in the handler, not the resolver.
internal sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ErrorOr<ProductDto>>
{
    public async Task<ErrorOr<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);

        return product is null
            ? CatalogErrors.ProductNotFound(request.Id)
            : product.ToDto();
    }
}
```

---

## Quick Start

```bash
# Install as dotnet new template
dotnet new install AniketJoshi.HotChocolateDdd.Template

# Create new project
dotnet new hc-ddd -n MyProject

# Start dependencies
cd MyProject
docker-compose up -d

# Run
dotnet run --project src/Api

# Open GraphQL IDE
# http://localhost:5000/graphql
```

Or clone directly:

```bash
git clone https://github.com/aniketljoshi/hotchocolate-ddd-cqrs-template
cd hotchocolate-ddd-cqrs-template
docker-compose up -d
dotnet run --project src/Api
```

---

## Project Structure

```
src/
├── Domain/          ← Pure C#. Zero external dependencies.
│                      AggregateRoot, ValueObject, IDomainEvent.
│
├── Application/     ← MediatR commands, queries, pipeline behaviors.
│                      DomainEventDispatchBehavior wires Outbox after SaveChanges.
│
├── Infrastructure/  ← EF Core, OutboxProcessor, repositories.
│                      ApplicationDbContext intercepts SaveChanges → writes OutboxMessage.
│
└── Api/             ← HotChocolate v15 resolvers (thin layer only).
                       Types, Inputs, Payloads, DataLoaders, Authorization.

tests/
├── Domain.Tests/        ← pure unit tests, zero infrastructure
├── Application.Tests/   ← handler tests with mocks
├── Architecture.Tests/  ← NetArchTest enforces layer boundaries
└── Integration.Tests/   ← full GraphQL request → Postgres via Testcontainers
```

---

## What's Included

| Feature | Status |
|---|---|
| HotChocolate v15 + `QueryContext<T>` | ✅ |
| .NET 10 LTS | ✅ |
| MediatR pipeline (validation + logging + domain events) | ✅ |
| DDD aggregate roots with domain event collection | ✅ |
| Outbox Pattern (background processor) | ✅ |
| ErrorOr result pattern — no exceptions as flow control | ✅ |
| FluentValidation in MediatR pipeline | ✅ |
| DataLoaders — N+1 problem solved + test proving it | ✅ |
| Field-level authorization example | ✅ |
| Architecture tests (NetArchTest) | ✅ |
| Integration tests (Testcontainers + real Postgres) | ✅ |
| OpenTelemetry hooks (GraphQL → MediatR → Outbox) | ✅ |
| `dotnet new` template on NuGet | 🔜 Roadmap |
| Architecture Decision Records (ADRs) | ✅ |

---

## Comparison

| | **This template** | Conference Planner | Jason Taylor |
|---|---|---|---|
| HotChocolate version | **v15** ✅ | v13 ❌ | No GraphQL |
| .NET version | **10 LTS** ✅ | 6 ❌ | 9 |
| Outbox Pattern | ✅ | ❌ | ❌ |
| Pure domain events | ✅ | ❌ | ⚠️ partial |
| Architecture tests | ✅ | ❌ | ✅ |
| DataLoader with N+1 test | ✅ | ⚠️ | ❌ |
| Field authorization | ✅ | ❌ | ❌ |
| `dotnet new` on NuGet | 🔜 | ❌ | ✅ |
| Last updated | **2025** | 2023 | Active |

---

## Roadmap

- [x] HotChocolate v15 + .NET 10 LTS
- [x] Outbox Pattern
- [x] Architecture tests
- [x] Domain events without MediatR dependency
- [ ] `dotnet new` template packaging on NuGet
- [ ] Keycloak / OIDC auth sample
- [ ] Subscriptions backed by domain events
- [ ] OpenTelemetry full trace with Jaeger screenshot
- [ ] Second bounded context example

---

## Contributing

Contributions are welcome. Please check the open issues before starting work.

Good entry points: [good first issue](../../labels/good%20first%20issue)

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

---

## Author

**Aniket Joshi** — Software Developer | Solution Designer | Software Architect
[aniketj.dev](https://aniketj.dev) · [LinkedIn](https://linkedin.com/in/aniketljoshi999) · [GitHub](https://github.com/aniketljoshi)

Built from production experience shipping HotChocolate + DDD at enterprise scale.

---

⭐ If this saved you time, a star helps other .NET developers find it.
