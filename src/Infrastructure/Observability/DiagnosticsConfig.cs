using HotChocolateDddCqrsTemplate.Application.Common.Observability;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Observability;

public static class DiagnosticsConfig
{
    public static IServiceCollection AddTemplateObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var enabled = configuration.GetValue("OpenTelemetry:Enabled", false);

        if (!enabled)
        {
            return services;
        }

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(TemplateTelemetry.ServiceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource(
                        TemplateTelemetry.ApiSourceName,
                        TemplateTelemetry.ApplicationSourceName,
                        TemplateTelemetry.InfrastructureSourceName)
                    .AddOtlpExporter(options =>
                    {
                        var endpoint = configuration["OpenTelemetry:OtlpEndpoint"];

                        if (!string.IsNullOrWhiteSpace(endpoint))
                        {
                            options.Endpoint = new Uri(endpoint);
                        }
                    });
            });

        return services;
    }
}
