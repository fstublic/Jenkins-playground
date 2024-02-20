namespace JenkinsPlayground.Server.Helpers;

public class IdProvider
{
    public static Guid NewId()
    {
        return RT.Comb.Provider.PostgreSql.Create();
    }
}