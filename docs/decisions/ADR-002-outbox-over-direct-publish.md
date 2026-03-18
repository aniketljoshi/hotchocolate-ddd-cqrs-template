# ADR-002: Outbox Pattern Over Direct Event Publishing

## Status

Accepted

## Context

After an aggregate is persisted, the application needs to publish domain events so that side-effects (email, search indexing, cache invalidation) can run. The simplest approach is to call `IMediator.Publish()` immediately after `SaveChangesAsync()`.

The problem: if the application crashes, is restarted, or the network drops between `SaveChangesAsync()` and `Publish()`, the event is lost silently. There is no exception, no log entry, and no retry. The aggregate is saved but the downstream side-effect never fires.

```csharp
// ❌ Silent data loss on crash
await _dbContext.SaveChangesAsync();
await _mediator.Publish(new ProductCreatedEvent(product.Id)); // lost if crash here
```

## Decision

Domain events are serialized into an `OutboxMessage` row and saved **in the same database transaction** as the aggregate. A background service (`OutboxProcessor`) polls the outbox table, deserializes each message, and publishes it as a MediatR notification.

```csharp
// ✅ Crash-safe — both saved atomically
await _dbContext.SaveChangesAsync(); // Product row + OutboxMessage row in one transaction
```

## Consequences

### Positive

- **Guaranteed at-least-once delivery** — if the app crashes, unprocessed outbox messages are picked up on restart.
- **Atomic consistency** — the event is either saved with the aggregate or not at all.
- **Auditable** — the outbox table serves as an event log with timestamps and processing status.
- **Framework-agnostic** — works with any message bus; the outbox is the abstraction boundary.

### Negative

- **Eventual consistency** — side-effects run with a small delay (default: 5-second polling interval).
- **At-least-once semantics** — handlers must be idempotent. A message may be delivered more than once if the processor crashes after publishing but before marking the message as processed.
- **Additional database writes** — each domain event adds one `outbox_messages` row per transaction.

## Alternatives Considered

- **Direct publish after SaveChanges** — rejected due to silent event loss on crashes.
- **Change Data Capture (CDC)** — considered but adds infrastructure complexity (Debezium, Kafka) beyond what a template should demonstrate.
- **Transactional outbox via EF Core interceptors** — a valid pattern but harder to understand; we chose explicit `IOutboxWriter` for clarity.
