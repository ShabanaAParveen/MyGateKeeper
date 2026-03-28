using AuthServer.Entity;

namespace AuthServer.Repositories
{
    public interface IUserRepository
    {
        UserInfo? GetUser(string username, string password);
    }
}
