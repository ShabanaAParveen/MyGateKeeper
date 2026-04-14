using AuthorizationServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationServer.DatabaseContext
{
    public class AuthZDbContext : DbContext
    {
        public AuthZDbContext(DbContextOptions<AuthZDbContext> options)
            : base(options) { }

        public DbSet<UserRole> UserRoles { get; set; }
    }
}
