using System.ComponentModel.DataAnnotations;
using JenkinsPlayground.Server.Helpers;

namespace JenkinsPlayground.Server.Tenants;

public class Tenant
{
    public Guid Id { get; set; } = IdProvider.NewId();
    [Required]
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}