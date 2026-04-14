using AuthorizationServer.Entities;
using AuthorizationServer.Models;

namespace AuthorizationServer.Repositories
{
    public interface IAuthorizationRepository
    {
        List<RoleDefinition> GetRoles(string code);
        DashboardAuthorizationContextDto? GetDashboardContext(string userCode);
    }
}
