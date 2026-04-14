using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MyGateKeeper.Services
{
    public class DashboardOrchestrationService(IHttpClientFactory httpClientFactory)
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<DashboardAuthContextResponseDto?> BuildAsync(
            string accessToken,
            ClaimsPrincipal user,
            CancellationToken cancellationToken)
        {
            var authzClient = _httpClientFactory.CreateClient("authz");
            authzClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var authzContext = await authzClient.GetFromJsonAsync<DashboardAuthorizationContextDto>(
                "/authz/dashboard-context",
                cancellationToken);

            if (authzContext is null)
            {
                return null;
            }

            var applicationCodes = authzContext.Roles
                .SelectMany(x => x.AllowedApps)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            var resourceClient = _httpClientFactory.CreateClient("resource");
            var query = string.Join("&", applicationCodes.Select(code => $"codes={Uri.EscapeDataString(code)}"));
            var applications = applicationCodes.Length == 0
                ? new List<ApplicationDto>()
                : await resourceClient.GetFromJsonAsync<List<ApplicationDto>>(
                    $"/resource/applications?{query}",
                    cancellationToken) ?? new List<ApplicationDto>();

            var userCode = user.FindFirst("userCode")?.Value ?? authzContext.UserCode;
            var displayName = user.FindFirst("displayName")?.Value
                ?? user.Identity?.Name
                ?? userCode;

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
