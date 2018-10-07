namespace TCM.WebAPI.Tasks
{
    public interface IBearerTokenExpirationTask
    {
        bool BearerTokenExpired(string token);
    }
}