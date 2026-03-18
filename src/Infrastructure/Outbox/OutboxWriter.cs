using System.Text.Json;
using HotChocolateDddCqrsTemplate.Application.Common.Interfaces;
using HotChocolateDddCqrsTemplate.Domain.Common;
using HotChocolateDddCqrsTemplate.Infrastructure.Persistence;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Outbox;

public sealed class OutboxWriter(ApplicationDbContext dbContext) : IOutboxWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public Task AddDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = dbContext.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(entry => entry.Entity)
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList();

        if (domainEvents.Count == 0)
        {
            return Task.CompletedTask;
        }

        var outboxMessages = domainEvents.Select(CreateOutboxMessage);
        dbContext.OutboxMessages.AddRange(outboxMessages);

        foreach (var aggregate in dbContext.ChangeTracker.Entries<AggregateRoot>().Select(entry => entry.Entity))
        {
            aggregate.ClearDomainEvents();
        }

        return Task.CompletedTask;
    }

    private static OutboxMessage CreateOutboxMessage(IDomainEvent domainEvent)
    {
        return OutboxMessage.Create(
            domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
            JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), SerializerOptions),
            domainEvent.OccurredOnUtc);
    }
}
