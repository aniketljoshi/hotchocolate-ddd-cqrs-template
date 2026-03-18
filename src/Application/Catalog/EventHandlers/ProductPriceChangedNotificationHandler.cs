using HotChocolateDddCqrsTemplate.Application.Catalog.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.EventHandlers;

public sealed class ProductPriceChangedNotificationHandler(ILogger<ProductPriceChangedNotificationHandler> logger)
    : INotificationHandler<ProductPriceChangedNotification>
{
    public Task Handle(ProductPriceChangedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Processed ProductPriceChangedDomainEvent for product {ProductId}: {OldPrice} -> {NewPrice} {Currency}",
            notification.DomainEvent.ProductId,
            notification.DomainEvent.OldPrice,
            notification.DomainEvent.NewPrice,
            notification.DomainEvent.Currency);

        return Task.CompletedTask;
    }
}
