using Microsoft.AspNetCore.Http;

namespace InsightTelemetryCore.Sessions;

public sealed record SessionContext(
    string? SessionId,
    string? UserAgent,
    string? RemoteAddress)
{
    public static SessionContext Empty { get; } = new(null, null, null);

    public static SessionContext FromHttpContext(HttpContext context)
        => new(
            context.Request.Cookies.TryGetValue(".AspNetCore.Session", out var sessionId) ? sessionId : null,
            context.Request.Headers["User-Agent"].ToString(),
            context.Connection.RemoteIpAddress?.ToString());
}
