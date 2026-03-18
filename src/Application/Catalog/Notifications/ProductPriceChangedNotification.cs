using HotChocolateDddCqrsTemplate.Domain.Catalog.Events;
using MediatR;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.Notifications;

public sealed record ProductPriceChangedNotification(ProductPriceChangedDomainEvent DomainEvent) : INotification;
