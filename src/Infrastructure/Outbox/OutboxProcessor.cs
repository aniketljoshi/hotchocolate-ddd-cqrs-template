using System.Diagnostics;
using System.Text.Json;
using HotChocolateDddCqrsTemplate.Application.Catalog.Notifications;
using HotChocolateDddCqrsTemplate.Application.Common.Observability;
using HotChocolateDddCqrsTemplate.Domain.Catalog.Events;
using HotChocolateDddCqrsTemplate.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Outbox;

public sealed class OutboxProcessor(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessagesAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unhandled error while processing outbox messages.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
    {
        using var batchActivity = TemplateTelemetry.InfrastructureActivitySource.StartActivity(
            "outbox.poll",
            ActivityKind.Consumer);

        using var scope = serviceScopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        var messages = await dbContext.OutboxMessages
            .Where(message => message.ProcessedOnUtc == null)
            .OrderBy(message => message.OccurredOnUtc)
            .Take(50)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            return;
        }

        foreach (var message in messages)
        {
            using var messageActivity = message.TryGetParentContext(out var parentContext)
                ? TemplateTelemetry.InfrastructureActivitySource.StartActivity(
                    "outbox.process_message",
                    ActivityKind.Consumer,
                    parentContext)
                : TemplateTelemetry.InfrastructureActivitySource.StartActivity(
                    "outbox.process_message",
                    ActivityKind.Consumer);

            messageActivity?.SetTag("outbox.message_id", message.Id);
            messageActivity?.SetTag("outbox.message_type", message.Type);

            try
            {
                var notification = DeserializeNotification(message);

                if (notification is null)
                {
                    message.MarkFailed($"Unknown outbox message type '{message.Type}'.");
                    continue;
                }

                await publisher.Publish(notification, cancellationToken);
                message.MarkProcessed(DateTime.UtcNow);
                messageActivity?.SetTag("outbox.status", "processed");
            }
            catch (Exception exception)
            {
                message.MarkFailed(exception.Message);
                messageActivity?.SetTag("outbox.status", "failed");
                messageActivity?.SetTag("outbox.error", exception.Message);
                logger.LogError(exception, "Failed to process outbox message {OutboxMessageId}", message.Id);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static INotification? DeserializeNotification(OutboxMessage message)
    {
        if (message.Type == typeof(ProductCreatedDomainEvent).FullName)
        {
            var domainEvent = JsonSerializer.Deserialize<ProductCreatedDomainEvent>(message.Content, SerializerOptions);
            return domainEvent is null ? null : new ProductCreatedNotification(domainEvent);
        }

        if (message.Type == typeof(ProductPriceChangedDomainEvent).FullName)
        {
            var domainEvent = JsonSerializer.Deserialize<ProductPriceChangedDomainEvent>(message.Content, SerializerOptions);
            return domainEvent is null ? null : new ProductPriceChangedNotification(domainEvent);
        }

        return null;
    }
}
