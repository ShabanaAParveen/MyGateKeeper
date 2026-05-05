namespace InsightTelemetryCore.Correlations;

public interface ICorrelationContextAccessor
{
    CorrelationContext? Current { get; }

    IDisposable BeginScope(CorrelationContext context);
}
