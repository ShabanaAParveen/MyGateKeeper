using Microsoft.EntityFrameworkCore;
using ResourceServer.Models;

namespace ResourceServer.DatabaseContext
{
    public class ResourceDBContext: DbContext
    {
       public ResourceDBContext(DbContextOptions<ResourceDBContext> options)
                : base(options) { }

       public DbSet<Application> Applications { get; set; }
        
    }
}
