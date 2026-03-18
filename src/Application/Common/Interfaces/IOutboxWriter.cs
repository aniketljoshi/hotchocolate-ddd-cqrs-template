namespace HotChocolateDddCqrsTemplate.Application.Common.Interfaces;

public interface IOutboxWriter
{
    Task AddDomainEventsAsync(CancellationToken cancellationToken);
}
