# ADR-001: Domain Events Do Not Implement `INotification`

## Status

Accepted

## Context

The template wants a domain layer that stays independent from delivery and framework concerns. If domain events directly implement MediatR's `INotification`, the domain project takes a dependency on an application library.

## Decision

Domain events implement the local `IDomainEvent` abstraction only.

The infrastructure/application pipeline serializes those events into the outbox and later maps them to MediatR notifications when the background processor republishes them.

## Consequences

- The domain stays framework-agnostic.
- Unit tests for aggregates do not need MediatR.
- Event publication requires an explicit mapping step in the outbox processor.

## Alternatives Considered

- Implement `INotification` directly in the domain:
  rejected because it couples the core model to MediatR.
- Publish events directly from handlers:
  rejected because it breaks the crash-safe outbox flow.
