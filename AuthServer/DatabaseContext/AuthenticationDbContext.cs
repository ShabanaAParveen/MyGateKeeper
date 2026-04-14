using AuthServer.Entity;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.DatabaseContext
{
    public class AuthenticationDbContext: DbContext
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
       : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccount>().ToTable("UserAccount");
        }
        public DbSet<UserAccount> UserAccounts { get; set; }
    }
}
