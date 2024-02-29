using System.ComponentModel.DataAnnotations;

namespace JenkinsPlayground.Server.Tenants;

public class TenantAddRequest
{
    [Required]
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}