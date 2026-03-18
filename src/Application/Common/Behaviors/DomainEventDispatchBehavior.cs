using HotChocolateDddCqrsTemplate.Application.Common.Interfaces;
using MediatR;

namespace HotChocolateDddCqrsTemplate.Application.Common.Behaviors;

public sealed class DomainEventDispatchBehavior<TRequest, TResponse>(
    IOutboxWriter outboxWriter,
    IApplicationDbContext applicationDbContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (request is not Common.Interfaces.ICommand<TResponse>)
        {
            return response;
        }

        await outboxWriter.AddDomainEventsAsync(cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return response;
    }
}
