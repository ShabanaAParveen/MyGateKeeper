namespace AuthorizationServer.Models
{
    public class DashboardAuthorizationContextDto
    {
        public string UserCode { get; set; } = string.Empty;
        public List<TenantDto> Tenants { get; set; } = new();
        public List<BusinessUnitDto> BusinessUnits { get; set; } = new();
        public List<RoleDto> Roles { get; set; } = new();
        public List<AssignmentDto> Assignments { get; set; } = new();
        public List<string> AllControls { get; set; } = new();
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
