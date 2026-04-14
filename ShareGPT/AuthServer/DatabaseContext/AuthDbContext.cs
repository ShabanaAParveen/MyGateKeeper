using AuthServer.Entity;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.DatabaseContext
{
    public class AuthDbContext: DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
       : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>().ToTable("UserInfo");
        }
        public DbSet<UserInfo> UserInfos { get; set; }
    }
}
