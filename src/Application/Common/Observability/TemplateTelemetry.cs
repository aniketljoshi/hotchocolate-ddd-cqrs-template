using System.Diagnostics;

namespace HotChocolateDddCqrsTemplate.Application.Common.Observability;

public static class TemplateTelemetry
{
    public const string ServiceName = "HotChocolateDddCqrsTemplate";
    public const string ApiSourceName = $"{ServiceName}/api";
    public const string ApplicationSourceName = $"{ServiceName}/application";
    public const string InfrastructureSourceName = $"{ServiceName}/infrastructure";

    public static readonly ActivitySource ApiActivitySource = new(ApiSourceName);
    public static readonly ActivitySource ApplicationActivitySource = new(ApplicationSourceName);
    public static readonly ActivitySource InfrastructureActivitySource = new(InfrastructureSourceName);
}
