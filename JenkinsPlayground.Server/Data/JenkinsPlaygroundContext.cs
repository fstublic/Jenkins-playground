using JenkinsPlayground.Server.Tenants;
using Microsoft.EntityFrameworkCore;

namespace JenkinsPlayground.Server.Data;

public class JenkinsPlaygroundContext : DbContext
{
    public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });
    
    public JenkinsPlaygroundContext(
        DbContextOptions<JenkinsPlaygroundContext> options
    )
        : base(options)
    {
        
    }
    
    public DbSet<Tenant> Tenants { get; set; }
}