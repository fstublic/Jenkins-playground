namespace JenkinsPlayground.Server.Helpers;

public class StaticConfiguration
{
    private static IConfiguration _configuration;

    public static string WebAppUrl =>
        Environment.GetEnvironmentVariable("WEB_APP_URL")
        ?? _configuration.GetValue<string>("WebAppUrl");

    public static string ConnectionStringsJenkinsPlaygroundDB =>
        Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? _configuration.GetValue<string>("ConnectionStrings:JenkinsPlaygroundDB");

    public static string AppSettingsSecret =>
        Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
        ?? _configuration.GetValue<string>("AppSettings:Secret");

    public static string AppSettingsExpirationDays =>
        Environment.GetEnvironmentVariable("JWT_EXPIRATION_DAYS")
        ?? _configuration.GetValue<string>("AppSettings:ExpirationDays");

    public static string ApiUrl =>
        Environment.GetEnvironmentVariable("API_URL") ?? _configuration.GetValue<string>("ApiUrl");

    public static string AppUrl =>
        Environment.GetEnvironmentVariable("APP_URL") ?? _configuration.GetValue<string>("AppUrl");

    public static string StorageUrl =>
        Environment.GetEnvironmentVariable("STORAGE_URL")
        ?? _configuration.GetValue<string>("StorageUrl");

    public static string StoragePath =>
        Environment.GetEnvironmentVariable("STORAGE_PATH")
        ?? _configuration.GetValue<string>("StoragePath");
    

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(ConnectionStringsJenkinsPlaygroundDB))
            errors.Add("ConnectionStringsJenkinsPlaygroundDB");

        if (string.IsNullOrWhiteSpace(AppSettingsSecret))
            errors.Add("AppSettingsSecret");

        if (errors.Count > 0)
            throw new Exception(string.Join(", ", errors));
    }
}