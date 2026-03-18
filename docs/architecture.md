# Architecture

The repository follows a layered structure:

- `Domain`: pure business model and domain events — zero external dependencies
- `Application`: commands, queries, validators, mappings, and MediatR behaviors
- `Infrastructure`: EF Core, repositories, outbox persistence, and background processing
- `Api`: HotChocolate v15 schema and thin resolvers

## System Diagram

```mermaid
flowchart TD
    Client(["Client\n(Browser / Mobile / Service)"])

    subgraph API["Api Layer — HotChocolate v15"]
        Resolver["GraphQL Resolver\n(thin — ISender only)"]
        DL["DataLoader\n(batches DB calls)"]
        Auth["Field Authorization\n(@authorize)"]
    end

    subgraph APP["Application Layer — MediatR"]
        Pipeline["MediatR Pipeline\nLogging → Validation → DomainEvents"]
        Handler["Command / Query Handler"]
    end

    subgraph DOMAIN["Domain Layer — Pure C#"]
        Aggregate["Aggregate Root\n(business logic)"]
        DomainEvent["IDomainEvent\n(no framework deps)"]
    end

    subgraph INFRA["Infrastructure Layer — EF Core"]
        DB[("PostgreSQL")]
        Outbox[("Outbox Table")]
        Processor["OutboxProcessor\n(background service)"]
        Repo["Repository"]
    end

    Client -->|"GraphQL request"| Resolver
    Resolver -->|"ISender.Send()"| Pipeline
    Pipeline --> Handler
    Handler --> Aggregate
    Aggregate -->|"raises"| DomainEvent
    Handler --> Repo
    Repo --> DB
    DomainEvent -->|"saved atomically\nwith aggregate"| Outbox
    Outbox -->|"polls every 5s"| Processor
    Processor -->|"INotificationHandler"| Handler
    Resolver --> DL
    Resolver --> Auth

    style DOMAIN fill:#f0f4ff,stroke:#4a6fa5
    style APP fill:#f0fff4,stroke:#2d6a4f
    style API fill:#fff8f0,stroke:#e07b00
    style INFRA fill:#fdf0f0,stroke:#c0392b
```

## Domain Model

```mermaid
classDiagram
    direction TB

    class AggregateRoot {
        <<abstract>>
        +List~IDomainEvent~ DomainEvents
        #RaiseDomainEvent(IDomainEvent event)
    }

    class Entity {
        <<abstract>>
        +Guid Id
    }

    class ValueObject {
        <<abstract>>
        #GetEqualityComponents() IEnumerable~object~
    }

    class IDomainEvent {
        <<interface>>
    }

    class Product {
        +Guid Id
        +string Name
        +Money Price
        +Money CostPrice
        +Sku Sku
        +bool IsArchived
        +Create(name, price, sku) Product$
        +UpdatePrice(Money newPrice)
        +Archive()
    }

    class Category {
        +Guid Id
        +string Name
        +string Slug
    }

    class Money {
        +decimal Amount
        +string Currency
        +Add(Money other) Money
        +IsZeroOrNegative() bool
    }

    class Sku {
        +string Value
        +IsValid() bool
    }

    class ProductCreatedDomainEvent {
        +Guid ProductId
        +string Name
        +decimal Price
        +DateTime OccurredAt
    }

    class ProductPriceChangedDomainEvent {
        +Guid ProductId
        +decimal OldPrice
        +decimal NewPrice
        +DateTime OccurredAt
    }

    AggregateRoot <|-- Product
    Entity <|-- Category
    Entity <|-- AggregateRoot
    ValueObject <|-- Money
    ValueObject <|-- Sku
    IDomainEvent <|.. ProductCreatedDomainEvent
    IDomainEvent <|.. ProductPriceChangedDomainEvent

    Product "1" --> "1" Money : Price
    Product "1" --> "1" Money : CostPrice
    Product "1" --> "1" Sku : Sku
    Product "1" --> "*" Category : belongs to
    Product ..> ProductCreatedDomainEvent : raises
    Product ..> ProductPriceChangedDomainEvent : raises
```

## Request Flow

See [request-flow.md](request-flow.md) for the full sequence diagram showing the lifecycle of a `createProduct` mutation from client through outbox processor.

## Key Decisions

| Decision | ADR |
|---|---|
| Domain events don't implement `INotification` | [ADR-001](decisions/ADR-001-domain-events-not-mediatr.md) |
| Outbox pattern over direct event publishing | [ADR-002](decisions/ADR-002-outbox-over-direct-publish.md) |
| Thin GraphQL resolvers as delivery adapters | [ADR-003](decisions/ADR-003-thin-graphql-resolvers.md) |
