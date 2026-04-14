using AuthorizationServer.DatabaseContext;
using AuthorizationServer.Entities;
using AuthorizationServer.Models;

namespace AuthorizationServer.Repositories
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly AuthZDbContext _context;

        public AuthorizationRepository(AuthZDbContext context)
        {
            _context = context;
        }

        public List<RoleDefinition> GetRoles(string code)
        {
            return _context.RoleDefinitions
                .Where(x => x.Code == code)
                .ToList();
        }

        public DashboardAuthorizationContextDto? GetDashboardContext(string userCode)
        {
            var assignments = _context.UserAssignments
                .Where(x => x.UserCode == userCode)
                .ToList();

            if (assignments.Count == 0)
            {
                return null;
            }

            var tenantCodes = assignments
                .Select(x => x.TenantCode)
                .Distinct()
                .ToList();

            var businessUnitCodes = assignments
                .Where(x => !string.IsNullOrWhiteSpace(x.BusinessUnitCode))
                .Select(x => x.BusinessUnitCode!)
                .Distinct()
                .ToList();

            var roleCodes = assignments
                .Select(x => x.RoleCode)
                .Distinct()
                .ToList();

            var tenants = _context.Tenants
                .Where(x => tenantCodes.Contains(x.Code) && x.IsActive)
                .Select(x => new TenantDto
                {
                    Id = x.Code,
                    Name = x.Name
                })
                .OrderBy(x => x.Id)
                .ToList();

            var businessUnits = _context.BusinessUnits
                .Where(x => businessUnitCodes.Contains(x.Code) && x.IsActive)
                .Select(x => new BusinessUnitDto
                {
                    Id = x.Code,
                    TenantId = x.TenantCode,
                    Name = x.Name
                })
                .OrderBy(x => x.Id)
                .ToList();

            var roleDefinitions = _context.RoleDefinitions
                .Where(x => roleCodes.Contains(x.Code) && x.IsActive)
                .OrderBy(x => x.Code)
                .ToList();

            var applicationGrants = _context.RoleApplicationGrants
                .Where(x => roleCodes.Contains(x.RoleCode) && x.IsActive)
                .ToList();

            var controlGrants = _context.RoleApplicationControlGrants
                .Where(x => roleCodes.Contains(x.RoleCode) && x.IsActive)
                .ToList();

            var roles = roleDefinitions
                .Select(role => new RoleDto
                {
                    Id = role.Code,
                    Name = role.Name,
                    Scope = role.ScopeType,
                    AllowedApps = applicationGrants
                        .Where(x => x.RoleCode == role.Code)
                        .Select(x => x.ApplicationCode)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList(),
                    Permissions = controlGrants
                        .Where(x => x.RoleCode == role.Code)
                        .GroupBy(x => x.ApplicationCode)
                        .ToDictionary(
                            group => group.Key,
                            group => group
                                .Select(x => x.ControlCode)
                                .Distinct()
                                .OrderBy(x => x)
                                .ToList())
                })
                .ToList();

            return new DashboardAuthorizationContextDto
            {
                UserCode = userCode,
                Tenants = tenants,
                BusinessUnits = businessUnits,
                Roles = roles,
                Assignments = assignments
                    .Select(x => new AssignmentDto
                    {
                        UserId = x.UserCode,
                        TenantId = x.TenantCode,
                        BuId = x.BusinessUnitCode ?? "GLOBAL",
                        RoleId = x.RoleCode
                    })
                    .OrderBy(x => x.TenantId)
                    .ThenBy(x => x.BuId)
                    .ThenBy(x => x.RoleId)
                    .ToList(),
                AllControls = controlGrants
                    .Select(x => x.ControlCode)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList()
            };
        }
    }
}
