using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthorizationServer.DatabaseContext
{
    public class AuthZDbContextFactory: IDesignTimeDbContextFactory<AuthZDbContext>
    {
        public AuthZDbContext CreateDbContext(string[] args) { 
            var optionsBuilder = new DbContextOptionsBuilder<AuthZDbContext>();
            optionsBuilder.UseSqlServer(
                "Data Source=DESKTOP-NG0BA81\\STUDIOX53;Initial Catalog=MyGateKeeperAuthZDb;Integrated Security=True;TrustServerCertificate=True;");
            return new AuthZDbContext(optionsBuilder.Options);
        }
    }
}
