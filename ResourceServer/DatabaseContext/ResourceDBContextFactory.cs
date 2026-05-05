using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ResourceServer.DatabaseContext
{
    public class ResourceDBContextFactory : IDesignTimeDbContextFactory<ResourceDBContext>
    {
        public ResourceDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ResourceDBContext>();
            optionsBuilder.UseSqlServer(
                "Data Source=DESKTOP-NG0BA81\\STUDIOX53;Initial Catalog=MyGateKeeperResourceDB;Integrated Security=True");
            return new ResourceDBContext(optionsBuilder.Options);
        }
    }
}
