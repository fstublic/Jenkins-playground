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
    
    private void disableCascadeDeletes(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        DbFormatter.SetDefaultValues(modelBuilder);
        DbFormatter.FormatTableNames(modelBuilder);
        DbFormatter.FormatColumnsSnakeCase(modelBuilder);
        disableCascadeDeletes(modelBuilder);
    }
}