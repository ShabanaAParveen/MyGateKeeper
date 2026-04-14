using AuthServer.Entity;

namespace AuthServer.Repositories
{
    public interface IUserRepository
    {
        UserAccount? GetUser(string username, string password);
    }
}
