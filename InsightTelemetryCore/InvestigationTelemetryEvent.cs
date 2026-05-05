using InsightTelemetryCore.Causations;
using InsightTelemetryCore.Correlations;
using InsightTelemetryCore.Sessions;
using InsightTelemetryCore.Trace;

namespace InsightTelemetryCore;

public sealed record InvestigationTelemetryEvent(
    string EventId,
    string Name,
    TelemetryCategory Category,
    TelemetrySeverity Severity,
    DateTimeOffset OccurredAtUtc,
    CorrelationContext Correlation,
    CausationContext Causation,
    SessionContext Session,
    InvestigationTraceContext Trace,
    IReadOnlyDictionary<string, object?> Attributes)
{
    public static InvestigationTelemetryEvent Create(
        string name,
        TelemetryCategory category,
        TelemetrySeverity severity,
        CorrelationContext correlation,
        CausationContext causation,
        SessionContext session,
        InvestigationTraceContext trace,
        IReadOnlyDictionary<string, object?>? attributes = null)
        => new(
            Guid.NewGuid().ToString("N"),
            name,
            category,
            severity,
            DateTimeOffset.UtcNow,
            correlation,
            causation,
            session,
            trace,
            attributes ?? EmptyAttributes.Value);
}

file static class EmptyAttributes
{
    public static readonly IReadOnlyDictionary<string, object?> Value = new Dictionary<string, object?>();
}
