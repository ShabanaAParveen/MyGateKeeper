namespace InsightTelemetryCore.Diagnostics;

public sealed record DiagnosticSignal(
    string Name,
    TelemetrySeverity Severity,
    string Message,
    string? Component = null,
    string? ErrorCode = null)
{
    public static DiagnosticSignal Create(
        string name,
        TelemetrySeverity severity,
        string message,
        string? component = null,
        string? errorCode = null)
        => new(name, severity, message, component, errorCode);

    public IReadOnlyDictionary<string, object?> ToAttributes()
        => new Dictionary<string, object?>
        {
            ["diagnostic.name"] = Name,
            ["diagnostic.message"] = Message,
            ["diagnostic.component"] = Component,
            ["diagnostic.error_code"] = ErrorCode
        };
}
