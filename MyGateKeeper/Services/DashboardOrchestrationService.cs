using InsightTelemetryCore;
using InsightTelemetryCore.Causations;
using InsightTelemetryCore.Correlations;
using InsightTelemetryCore.Sessions;
using InsightTelemetryCore.Trace;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MyGateKeeper.Services
{
    public class DashboardOrchestrationService(
        IHttpClientFactory httpClientFactory,
        InvestigationLogger investigationLogger,
        ICorrelationContextAccessor correlationAccessor)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly InvestigationLogger _investigationLogger = investigationLogger;
        private readonly ICorrelationContextAccessor _correlationAccessor = correlationAccessor;

        public async Task<DashboardAuthContextResponseDto?> BuildAsync(
            string accessToken,
            ClaimsPrincipal user,
            CancellationToken cancellationToken)
        {
            var correlation = _correlationAccessor.Current ?? CorrelationContext.New("dashboard.context");
            var userCode = user.FindFirst("userCode")?.Value;
            await WriteEventAsync(
                "dashboard.context.started",
                TelemetrySeverity.Information,
                correlation,
                new Dictionary<string, object?>
                {
                    ["user.code"] = userCode,
                    ["user.name"] = user.Identity?.Name
                },
                cancellationToken);

            var authzClient = _httpClientFactory.CreateClient("authz");
            using var authzRequest = new HttpRequestMessage(HttpMethod.Post, "/authz/dashboard-context");
            authzRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            authzRequest.Headers.TryAddWithoutValidation("X-Correlation-Id", correlation.CorrelationId);

            await WriteEventAsync(
                "dashboard.authz.requested",
                TelemetrySeverity.Information,
                correlation,
                new Dictionary<string, object?>
                {
                    ["target.service"] = "AuthorizationServer",
                    ["target.route"] = "/authz/dashboard-context"
                },
                cancellationToken);

            using var authzResponse = await authzClient.SendAsync(authzRequest, cancellationToken);
            await WriteEventAsync(
                "dashboard.authz.responded",
                authzResponse.IsSuccessStatusCode ? TelemetrySeverity.Information : TelemetrySeverity.Warning,
                correlation,
                new Dictionary<string, object?>
                {
                    ["target.service"] = "AuthorizationServer",
                    ["http.status_code"] = (int)authzResponse.StatusCode
                },
                cancellationToken);

            if (!authzResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var authzContext = await authzResponse.Content.ReadFromJsonAsync<DashboardAuthorizationContextDto>(
                cancellationToken);

            if (authzContext is null)
            {
                await WriteEventAsync(
                    "dashboard.authz.empty",
                    TelemetrySeverity.Warning,
                    correlation,
                    new Dictionary<string, object?>
                    {
                        ["reason"] = "AuthorizationServer returned an empty dashboard context."
                    },
                    cancellationToken);

                return null;
            }

            var applicationCodes = authzContext.Roles
                .SelectMany(x => x.AllowedApps)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            var resourceClient = _httpClientFactory.CreateClient("resource");
            var query = string.Join("&", applicationCodes.Select(code => $"codes={Uri.EscapeDataString(code)}"));
            List<ApplicationDto> applications;

            if (applicationCodes.Length == 0)
            {
                applications = new List<ApplicationDto>();
            }
            else
            {
                using var resourceRequest = new HttpRequestMessage(HttpMethod.Get, $"/resource/applications?{query}");
                resourceRequest.Headers.TryAddWithoutValidation("X-Correlation-Id", correlation.CorrelationId);

                await WriteEventAsync(
                    "dashboard.resource.requested",
                    TelemetrySeverity.Information,
                    correlation,
                    new Dictionary<string, object?>
                    {
                        ["target.service"] = "ResourceServer",
                        ["application.code_count"] = applicationCodes.Length
                    },
                    cancellationToken);

                using var resourceResponse = await resourceClient.SendAsync(resourceRequest, cancellationToken);
                await WriteEventAsync(
                    "dashboard.resource.responded",
                    resourceResponse.IsSuccessStatusCode ? TelemetrySeverity.Information : TelemetrySeverity.Warning,
                    correlation,
                    new Dictionary<string, object?>
                    {
                        ["target.service"] = "ResourceServer",
                        ["http.status_code"] = (int)resourceResponse.StatusCode
                    },
                    cancellationToken);

                if (!resourceResponse.IsSuccessStatusCode)
                {
                    return null;
                }

                applications = await resourceResponse.Content.ReadFromJsonAsync<List<ApplicationDto>>(
                    cancellationToken) ?? new List<ApplicationDto>();
            }

            userCode = userCode ?? authzContext.UserCode;
            var displayName = user.FindFirst("displayName")?.Value
                ?? user.Identity?.Name
                ?? userCode;

            await WriteEventAsync(
                "dashboard.context.completed",
                TelemetrySeverity.Information,
                correlation,
                new Dictionary<string, object?>
                {
                    ["tenant.count"] = authzContext.Tenants.Count,
                    ["business_unit.count"] = authzContext.BusinessUnits.Count,
                    ["role.count"] = authzContext.Roles.Count,
                    ["assignment.count"] = authzContext.Assignments.Count,
                    ["application.count"] = applications.Count,
                    ["control.count"] = authzContext.AllControls.Count
                },
                cancellationToken);

            return new DashboardAuthContextResponseDto
            {
                Users =
                [
                    new UserDto
                    {
                        Id = userCode,
                        Name = displayName
                    }
                ],
                Tenants = authzContext.Tenants,
                BusinessUnits = authzContext.BusinessUnits,
                Applications = applications,
                Roles = authzContext.Roles,
                Assignments = authzContext.Assignments,
                AllControls = authzContext.AllControls
            };
        }

        private ValueTask WriteEventAsync(
            string name,
            TelemetrySeverity severity,
            CorrelationContext correlation,
            IReadOnlyDictionary<string, object?> attributes,
            CancellationToken cancellationToken)
        {
            var telemetryEvent = InvestigationTelemetryEvent.Create(
                name,
                TelemetryCategory.Trace,
                severity,
                correlation,
                CausationContext.Root,
                SessionContext.Empty,
                InvestigationTraceContext.FromActivity(System.Diagnostics.Activity.Current),
                attributes);

            return _investigationLogger.WriteAsync(telemetryEvent, cancellationToken);
        }
    }

    public class DashboardAuthContextResponseDto
    {
        public List<UserDto> Users { get; set; } = new();
        public List<TenantDto> Tenants { get; set; } = new();
        public List<BusinessUnitDto> BusinessUnits { get; set; } = new();
        public List<ApplicationDto> Applications { get; set; } = new();
        public List<RoleDto> Roles { get; set; } = new();
        public List<AssignmentDto> Assignments { get; set; } = new();
        public List<string> AllControls { get; set; } = new();
    }

    public class DashboardAuthorizationContextDto
    {
        public string UserCode { get; set; } = string.Empty;
        public List<TenantDto> Tenants { get; set; } = new();
        public List<BusinessUnitDto> BusinessUnits { get; set; } = new();
        public List<RoleDto> Roles { get; set; } = new();
        public List<AssignmentDto> Assignments { get; set; } = new();
        public List<string> AllControls { get; set; } = new();
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class TenantDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class BusinessUnitDto
    {
        public string Id { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class ApplicationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string LaunchUrl { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public string? Maintainer { get; set; }
        public string? ContactEmail { get; set; }
    }

    public class RoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public List<string> AllowedApps { get; set; } = new();
        public Dictionary<string, List<string>> Permissions { get; set; } = new();
    }

    public class AssignmentDto
    {
        public string UserId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string BuId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
    }
}
