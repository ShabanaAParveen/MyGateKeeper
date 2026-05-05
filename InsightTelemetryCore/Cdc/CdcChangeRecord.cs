using InsightTelemetryCore.Causations;
using InsightTelemetryCore.Correlations;
using InsightTelemetryCore.Sessions;
using InsightTelemetryCore.Trace;

namespace InsightTelemetryCore.Cdc;

public sealed record CdcChangeRecord(
    string Source,
    string EntityName,
    string EntityKey,
    CdcOperation Operation,
    DateTimeOffset ChangedAtUtc,
    IReadOnlyDictionary<string, object?> Before,
    IReadOnlyDictionary<string, object?> After,
    string? TransactionId = null,
    long? Sequence = null)
{
    public InvestigationTelemetryEvent ToTelemetryEvent(
        CorrelationContext correlation,
        CausationContext? causation = null,
        SessionContext? session = null,
        InvestigationTraceContext? trace = null)
    {
        var attributes = new Dictionary<string, object?>
        {
            ["cdc.source"] = Source,
            ["cdc.entity"] = EntityName,
            ["cdc.key"] = EntityKey,
            ["cdc.operation"] = Operation.ToString(),
            ["cdc.changed_at_utc"] = ChangedAtUtc,
            ["cdc.transaction_id"] = TransactionId,
            ["cdc.sequence"] = Sequence,
            ["cdc.before"] = Before,
            ["cdc.after"] = After
        };

        return InvestigationTelemetryEvent.Create(
            $"cdc.{EntityName}.{Operation.ToString().ToLowerInvariant()}",
            TelemetryCategory.Cdc,
            TelemetrySeverity.Information,
            correlation,
            causation ?? CausationContext.Root,
            session ?? SessionContext.Empty,
            trace ?? InvestigationTraceContext.Empty,
            attributes);
    }
}
