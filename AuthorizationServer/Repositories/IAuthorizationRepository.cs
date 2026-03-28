namespace AuthorizationServer.Repositories
{
    public interface IAuthorizationRepository
    {
        List<string> GetRoles(int userId);
    }
}
