using HotChocolateDddCqrsTemplate.Application.Catalog.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotChocolateDddCqrsTemplate.Application.Catalog.EventHandlers;

public sealed class ProductCreatedNotificationHandler(ILogger<ProductCreatedNotificationHandler> logger)
    : INotificationHandler<ProductCreatedNotification>
{
    public Task Handle(ProductCreatedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Processed ProductCreatedDomainEvent for product {ProductId} ({ProductName})",
            notification.DomainEvent.ProductId,
            notification.DomainEvent.Name);

        return Task.CompletedTask;
    }
}
