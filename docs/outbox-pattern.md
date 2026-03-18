# Outbox Pattern

The template persists domain events as `OutboxMessage` rows in the same database transaction as the aggregate changes.

## Why

Publishing directly after `SaveChangesAsync` is fragile:

```csharp
await _dbContext.SaveChangesAsync();
await _publisher.Publish(new ProductCreatedDomainEvent(...));
```

If the process crashes between those two lines, the domain event is lost.

## Current Flow

1. Command handlers mutate aggregates and return.
2. `DomainEventDispatchBehavior` asks `IOutboxWriter` to serialize pending domain events into `outbox_messages`.
3. The same behavior then calls `SaveChangesAsync`, so aggregate changes and outbox rows commit together.
4. `OutboxProcessor` polls unprocessed rows, deserializes them back into notifications, and publishes them through MediatR.

This gives the template at-least-once delivery semantics for domain event processing inside the application boundary.
