# Architecture

The repository follows a layered structure:

- `Domain`: pure business model and domain events
- `Application`: commands, queries, validators, mappings, and MediatR behaviors
- `Infrastructure`: EF Core, repositories, outbox persistence, and background processing
- `Api`: HotChocolate schema and thin resolvers

## System Diagram

```mermaid
flowchart TD
    Client(["Client"])

    subgraph API["Api Layer"]
        Resolver["GraphQL Resolver"]
        DL["CategoryByIdDataLoader"]
    end

    subgraph APP["Application Layer"]
        Pipeline["MediatR Pipeline"]
        Handler["Command / Query Handler"]
    end

    subgraph DOMAIN["Domain Layer"]
        Aggregate["Product Aggregate"]
        DomainEvent["IDomainEvent"]
    end

    subgraph INFRA["Infrastructure Layer"]
        DB[("PostgreSQL")]
        Outbox[("Outbox Table")]
        Processor["OutboxProcessor"]
        Repo["Repositories"]
    end

    Client --> Resolver
    Resolver --> DL
    Resolver --> Pipeline
    Pipeline --> Handler
    Handler --> Aggregate
    Aggregate --> DomainEvent
    Handler --> Repo
    Repo --> DB
    DomainEvent --> Outbox
    Outbox --> Processor
```

## Domain Model

```mermaid
classDiagram
    class AggregateRoot {
        <<abstract>>
        +DomainEvents
        +ClearDomainEvents()
    }

    class Product {
        +Guid Id
        +string Name
        +Money Price
        +Sku Sku
        +Guid CategoryId
        +bool IsArchived
        +Create(...)
        +UpdatePrice(...)
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
    }

    class Sku {
        +string Value
    }

    class ProductCreatedDomainEvent
    class ProductPriceChangedDomainEvent

    AggregateRoot <|-- Product
    Product --> Money
    Product --> Sku
    Product ..> ProductCreatedDomainEvent
    Product ..> ProductPriceChangedDomainEvent
```
