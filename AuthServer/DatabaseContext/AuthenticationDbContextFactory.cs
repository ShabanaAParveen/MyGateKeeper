using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthServer.DatabaseContext
{
    public class AuthenticationDbContextFactory : IDesignTimeDbContextFactory<AuthenticationDbContext>
    {
        public AuthenticationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthenticationDbContext>();

            optionsBuilder.UseSqlServer(
    "Data Source=DESKTOP-NG0BA81\\STUDIOX53;Initial Catalog=MyGateKeeperAuthNDb;Integrated Security=True;TrustServerCertificate=True;");

            return new AuthenticationDbContext(optionsBuilder.Options);
        }
    }
}
