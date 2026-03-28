using AuthServer.DatabaseContext;
using AuthServer.Entity;

namespace AuthServer.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
        {
            _context = context;
        }

        public UserInfo? GetUser(string username, string password)
        {
            return _context.UserInfos.FirstOrDefault(u => u.UserName == username && u.Password == password);
        }
    }
}
