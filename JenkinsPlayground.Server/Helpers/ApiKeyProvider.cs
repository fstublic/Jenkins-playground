using AspNetCore.Authentication.ApiKey;
using JenkinsPlayground.Server.Data;

namespace JenkinsPlayground.Server.Helpers;

public class ApiKeyProvider : IApiKeyProvider
{
    private readonly JenkinsPlaygroundContext _context;

    public ApiKeyProvider(JenkinsPlaygroundContext context)
    {
        _context = context;
    }

    public Task<IApiKey> ProvideAsync(string key)
    {
        throw new UnauthorizedException();
    }
}