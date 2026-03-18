using HotChocolateDddCqrsTemplate.Domain.Common;

namespace HotChocolateDddCqrsTemplate.Domain.Catalog.Events;

public sealed record ProductCreatedDomainEvent(
    Guid ProductId,
    string Name,
    decimal Price,
    string Currency,
    DateTime OccurredOnUtc) : IDomainEvent;
