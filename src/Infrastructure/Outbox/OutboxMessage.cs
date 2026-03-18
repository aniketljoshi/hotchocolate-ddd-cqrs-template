using System.Diagnostics;

namespace HotChocolateDddCqrsTemplate.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
        Type = string.Empty;
        Content = string.Empty;
    }

    private OutboxMessage(Guid id, string type, string content, DateTime occurredOnUtc)
    {
        Id = id;
        Type = type;
        Content = content;
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid Id { get; private set; }

    public string Type { get; private set; }

    public string Content { get; private set; }

    public DateTime OccurredOnUtc { get; private set; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public string? Error { get; private set; }

    public string? TraceId { get; private set; }

    public string? SpanId { get; private set; }

    public byte? TraceFlags { get; private set; }

    public static OutboxMessage Create(
        string type,
        string content,
        DateTime occurredOnUtc,
        ActivityContext? activityContext = null)
    {
        var message = new OutboxMessage(Guid.NewGuid(), type, content, occurredOnUtc);

        if (activityContext is { TraceId: var traceId, SpanId: var spanId } context &&
            traceId != default &&
            spanId != default)
        {
            message.TraceId = traceId.ToString();
            message.SpanId = spanId.ToString();
            message.TraceFlags = (byte)context.TraceFlags;
        }

        return message;
    }

    public void MarkProcessed(DateTime processedOnUtc)
    {
        ProcessedOnUtc = processedOnUtc;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        Error = error;
    }

    public bool TryGetParentContext(out ActivityContext activityContext)
    {
        if (string.IsNullOrWhiteSpace(TraceId) ||
            string.IsNullOrWhiteSpace(SpanId))
        {
            activityContext = default;
            return false;
        }

        ActivityTraceId traceId;
        ActivitySpanId spanId;

        try
        {
            traceId = ActivityTraceId.CreateFromString(TraceId.AsSpan());
            spanId = ActivitySpanId.CreateFromString(SpanId.AsSpan());
        }
        catch (FormatException)
        {
            activityContext = default;
            return false;
        }

        activityContext = new ActivityContext(
            traceId,
            spanId,
            (ActivityTraceFlags)(TraceFlags ?? (byte)ActivityTraceFlags.Recorded));

        return true;
    }
}
