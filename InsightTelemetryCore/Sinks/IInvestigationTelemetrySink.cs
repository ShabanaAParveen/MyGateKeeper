namespace InsightTelemetryCore.Sinks;

public interface IInvestigationTelemetrySink
{
    ValueTask WriteAsync(InvestigationTelemetryEvent telemetryEvent, CancellationToken cancellationToken = default);
}
