using System.Text.Json;

namespace InsightTelemetryCore.Sinks;

public sealed class FileSystemInvestigationTelemetrySink : IInvestigationTelemetrySink
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly string _directoryPath;

    public FileSystemInvestigationTelemetrySink(string directoryPath)
    {
        _directoryPath = directoryPath;
        Directory.CreateDirectory(_directoryPath);
    }

    public async ValueTask WriteAsync(InvestigationTelemetryEvent telemetryEvent, CancellationToken cancellationToken = default)
    {
        var safeCorrelationId = SanitizeFileName(telemetryEvent.Correlation.CorrelationId);
        var filePath = Path.Combine(_directoryPath, $"{safeCorrelationId}.jsonl");
        var line = JsonSerializer.Serialize(telemetryEvent, JsonOptions);

        await File.AppendAllTextAsync(filePath, line + Environment.NewLine, cancellationToken);
    }

    private static string SanitizeFileName(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var cleaned = new string(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
        return string.IsNullOrWhiteSpace(cleaned) ? "unknown-correlation" : cleaned;
    }
}
