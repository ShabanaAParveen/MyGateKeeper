using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace InsightTelemetryCore.Trace;

public sealed record InvestigationTraceContext(
    string? TraceId,
    string? SpanId,
    string? ParentSpanId,
    string? RequestTraceIdentifier)
{
    public static InvestigationTraceContext Empty { get; } = new(null, null, null, null);

    public static InvestigationTraceContext FromActivity(Activity? activity)
        => activity is null
            ? Empty
            : new(
                activity.TraceId.ToString(),
                activity.SpanId.ToString(),
                activity.ParentSpanId.ToString(),
                null);

    public static InvestigationTraceContext FromHttpContext(HttpContext context)
    {
        var activity = Activity.Current;
        return new(
            activity?.TraceId.ToString(),
            activity?.SpanId.ToString(),
            activity?.ParentSpanId.ToString(),
            context.TraceIdentifier);
    }
}
