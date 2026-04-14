using AuthorizationServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationServer.DatabaseContext
{
    public class AuthZDbContext : DbContext
    {
        public AuthZDbContext(DbContextOptions<AuthZDbContext> options)
            : base(options) { }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<BusinessUnit> BusinessUnits { get; set; }
        public DbSet<RoleDefinition> RoleDefinitions { get; set; }
        public DbSet<UserAssignment> UserAssignments { get; set; }
        public DbSet<RoleApplicationGrant> RoleApplicationGrants { get; set; }
        public DbSet<RoleApplicationControlGrant> RoleApplicationControlGrants { get; set; }
    }
}
