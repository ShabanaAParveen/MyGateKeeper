using AuthServer.DatabaseContext;
using AuthServer.Entity;

namespace AuthServer.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly AuthenticationDbContext _context;

        public UserRepository(AuthenticationDbContext context)
        {
            _context = context;
        }

        public UserAccount? GetUser(string username, string password)
        {
            return _context.UserAccounts.FirstOrDefault(u => u.UserName == username && u.PwdHash == password);
        }
    }
}
