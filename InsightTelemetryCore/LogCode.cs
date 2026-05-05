using InsightTelemetryCore.Causations;
using InsightTelemetryCore.Correlations;
using InsightTelemetryCore.Diagnostics;
using InsightTelemetryCore.Sessions;
using InsightTelemetryCore.Sinks;
using InsightTelemetryCore.Trace;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace InsightTelemetryCore;

public sealed class InvestigationLogger
{
    private readonly IInvestigationTelemetrySink _sink;

    public InvestigationLogger(IInvestigationTelemetrySink sink)
    {
        _sink = sink;
    }

    public ValueTask WriteAsync(InvestigationTelemetryEvent telemetryEvent, CancellationToken cancellationToken = default)
        => _sink.WriteAsync(telemetryEvent, cancellationToken);

    public ValueTask DiagnosticAsync(
        string name,
        TelemetrySeverity severity,
        string message,
        CorrelationContext correlation,
        CancellationToken cancellationToken = default)
    {
        var signal = DiagnosticSignal.Create(name, severity, message);
        var telemetryEvent = InvestigationTelemetryEvent.Create(
            name,
            TelemetryCategory.Diagnostics,
            severity,
            correlation,
            CausationContext.Root,
            SessionContext.Empty,
            InvestigationTraceContext.FromActivity(Activity.Current),
            signal.ToAttributes());

        return WriteAsync(telemetryEvent, cancellationToken);
    }
}

public sealed class InvestigationLogWriterWorker : BackgroundService
{
    private readonly ChannelInvestigationTelemetrySink _sink;

    public InvestigationLogWriterWorker(ChannelInvestigationTelemetrySink sink)
    {
        _sink = sink;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => _sink.FlushToFileAsync(stoppingToken);
}

public sealed class AnalyticsLogger
{
    private readonly RequestDelegate _next;
    private readonly InvestigationLogger _logger;
    private readonly ICorrelationContextAccessor _correlationAccessor;

    public AnalyticsLogger(
        RequestDelegate next,
        InvestigationLogger logger,
        ICorrelationContextAccessor correlationAccessor)
    {
        _next = next;
        _logger = logger;
        _correlationAccessor = correlationAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startedAt = Stopwatch.GetTimestamp();
        var correlation = ResolveCorrelation(context);
        using var scope = _correlationAccessor.BeginScope(correlation);
        Exception? failure = null;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            failure = ex;
            throw;
        }
        finally
        {
            var duration = Stopwatch.GetElapsedTime(startedAt);
            var severity = failure is null ? TelemetrySeverity.Information : TelemetrySeverity.Error;
            var attributes = new Dictionary<string, object?>
            {
                ["http.method"] = context.Request.Method,
                ["http.path"] = context.Request.Path.Value,
                ["http.status_code"] = context.Response.StatusCode,
                ["duration_ms"] = duration.TotalMilliseconds,
                ["exception.type"] = failure?.GetType().FullName,
                ["exception.message"] = failure?.Message
            };

            var telemetryEvent = InvestigationTelemetryEvent.Create(
                "http.request",
                TelemetryCategory.Trace,
                severity,
                correlation,
                CausationContext.Root,
                SessionContext.FromHttpContext(context),
                InvestigationTraceContext.FromHttpContext(context),
                attributes);

            await _logger.WriteAsync(telemetryEvent, context.RequestAborted);
        }
    }

    private static CorrelationContext ResolveCorrelation(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue("X-Correlation-Id", out var header)
            ? header.ToString()
            : context.TraceIdentifier;

        context.Response.Headers["X-Correlation-Id"] = correlationId;

        return new CorrelationContext(
            correlationId,
            TenantId: context.User.FindFirst("tenant_id")?.Value,
            SubjectId: context.User.Identity?.Name,
            OperationName: $"{context.Request.Method} {context.Request.Path}");
    }
}
