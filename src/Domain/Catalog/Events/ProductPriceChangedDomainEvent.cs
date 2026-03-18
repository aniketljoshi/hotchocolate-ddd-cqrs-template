using HotChocolateDddCqrsTemplate.Domain.Common;

namespace HotChocolateDddCqrsTemplate.Domain.Catalog.Events;

public sealed record ProductPriceChangedDomainEvent(
    Guid ProductId,
    decimal OldPrice,
    decimal NewPrice,
    string Currency,
    DateTime OccurredOnUtc) : IDomainEvent;
