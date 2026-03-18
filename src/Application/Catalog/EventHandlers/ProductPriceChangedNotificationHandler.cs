using System.Diagnostics;
using HotChocolateDddCqrsTemplate.Application.Catalog.Notifications;
using HotChocolateDddCqrsTemplate.Application.Common.Observability;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.EventHandlers;

public sealed class ProductPriceChangedNotificationHandler(ILogger<ProductPriceChangedNotificationHandler> logger)
    : INotificationHandler<ProductPriceChangedNotification>
{
    public Task Handle(ProductPriceChangedNotification notification, CancellationToken cancellationToken)
    {
        using var activity = TemplateTelemetry.ApplicationActivitySource.StartActivity(
            "notification.product_price_changed",
            ActivityKind.Internal);

        activity?.SetTag("product.id", notification.DomainEvent.ProductId);
        activity?.SetTag("product.old_price", notification.DomainEvent.OldPrice);
        activity?.SetTag("product.new_price", notification.DomainEvent.NewPrice);
        activity?.SetTag("product.currency", notification.DomainEvent.Currency);

        logger.LogInformation(
            "Processed ProductPriceChangedDomainEvent for product {ProductId}: {OldPrice} -> {NewPrice} {Currency}",
            notification.DomainEvent.ProductId,
            notification.DomainEvent.OldPrice,
            notification.DomainEvent.NewPrice,
            notification.DomainEvent.Currency);

        return Task.CompletedTask;
    }
}
