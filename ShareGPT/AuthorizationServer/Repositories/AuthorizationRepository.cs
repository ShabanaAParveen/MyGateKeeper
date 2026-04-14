using AuthorizationServer.DatabaseContext;

namespace AuthorizationServer.Repositories
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly AuthZDbContext _context;

        public AuthorizationRepository(AuthZDbContext context)
        {
            _context = context;
        }

        public List<string> GetRoles(int userId)
        {
            return _context.UserRoles
                .Where(x => x.UserId == userId)
                .Select(x => x.Role)
                .ToList();
        }
    }
}
