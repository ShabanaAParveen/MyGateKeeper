using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthServer.DatabaseContext
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

            optionsBuilder.UseSqlServer(
    "Data Source=DESKTOP-NG0BA81\\STUDIOX53;Initial Catalog=MyGateKeeperAuthDb;Integrated Security=True;TrustServerCertificate=True;");

            return new AuthDbContext(optionsBuilder.Options);
        }
    }
}
