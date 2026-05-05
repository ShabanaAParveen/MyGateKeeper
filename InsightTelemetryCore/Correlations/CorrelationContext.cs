namespace InsightTelemetryCore.Correlations;

public sealed record CorrelationContext(
    string CorrelationId,
    string? TenantId = null,
    string? SubjectId = null,
    string? OperationName = null)
{
    public static CorrelationContext New(string? operationName = null)
        => new(Guid.NewGuid().ToString("N"), OperationName: operationName);
}
