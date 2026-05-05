using Microsoft.EntityFrameworkCore;
using ResourceServer.Models;

namespace ResourceServer.DatabaseContext
{
    public class ResourceDBContext: DbContext
    {
       public ResourceDBContext(DbContextOptions<ResourceDBContext> options)
                : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>().ToTable("Application");
        }
        public DbSet<Application> Applications { get; set; }
        
    }
}
