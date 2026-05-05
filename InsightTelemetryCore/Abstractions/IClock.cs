namespace InsightTelemetryCore.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
