namespace InsightTelemetryCore.Causations;

public sealed record CausationContext(
    string? ParentEventId,
    string? CauseEventId,
    string Relation)
{
    public static CausationContext Root { get; } = new(null, null, "root");

    public static CausationContext CausedBy(string parentEventId, string causeEventId)
        => new(parentEventId, causeEventId, "caused_by");
}
