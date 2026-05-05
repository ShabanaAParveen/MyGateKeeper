using System.Text.Json;
using System.Threading.Channels;

namespace InsightTelemetryCore.Sinks;

public sealed class ChannelInvestigationTelemetrySink : IInvestigationTelemetrySink
{
    private readonly Channel<InvestigationTelemetryEvent> _channel;
    private readonly string _filePath;

    public ChannelInvestigationTelemetrySink(string filePath = "investigation_report.log")
    {
        _filePath = filePath;
        _channel = Channel.CreateUnbounded<InvestigationTelemetryEvent>(
            new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false
            });
    }

    public ValueTask WriteAsync(InvestigationTelemetryEvent telemetryEvent, CancellationToken cancellationToken = default)
    {
        if (!_channel.Writer.TryWrite(telemetryEvent))
        {
            return ValueTask.FromException(new InvalidOperationException("Investigation telemetry channel is closed."));
        }

        return ValueTask.CompletedTask;
    }

    public async Task FlushToFileAsync(CancellationToken cancellationToken)
    {
        await foreach (var telemetryEvent in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            var line = JsonSerializer.Serialize(telemetryEvent);
            await File.AppendAllTextAsync(_filePath, line + Environment.NewLine, cancellationToken);
        }
    }
}
