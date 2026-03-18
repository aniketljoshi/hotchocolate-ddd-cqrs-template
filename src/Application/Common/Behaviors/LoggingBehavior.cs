using System.Diagnostics;
using HotChocolateDddCqrsTemplate.Application.Common.Observability;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotChocolateDddCqrsTemplate.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);

        using var activity = TemplateTelemetry.ApplicationActivitySource.StartActivity(
            $"{typeof(TRequest).Name}.handle",
            ActivityKind.Internal);

        activity?.SetTag("request.type", typeof(TRequest).FullName);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        activity?.SetTag("request.elapsed_ms", stopwatch.ElapsedMilliseconds);

        logger.LogInformation(
            "Handled {RequestName} in {ElapsedMilliseconds} ms",
            typeof(TRequest).Name,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}
