using System.Diagnostics;
using HotChocolateDddCqrsTemplate.Application.Catalog.Notifications;
using HotChocolateDddCqrsTemplate.Application.Common.Observability;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.EventHandlers;

public sealed class ProductCreatedNotificationHandler(ILogger<ProductCreatedNotificationHandler> logger)
    : INotificationHandler<ProductCreatedNotification>
{
    public Task Handle(ProductCreatedNotification notification, CancellationToken cancellationToken)
    {
        using var activity = TemplateTelemetry.ApplicationActivitySource.StartActivity(
            "notification.product_created",
            ActivityKind.Internal);

        activity?.SetTag("product.id", notification.DomainEvent.ProductId);
        activity?.SetTag("product.name", notification.DomainEvent.Name);

        logger.LogInformation(
            "Processed ProductCreatedDomainEvent for product {ProductId} ({ProductName})",
            notification.DomainEvent.ProductId,
            notification.DomainEvent.Name);

        return Task.CompletedTask;
    }
}
