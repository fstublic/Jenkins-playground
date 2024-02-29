using AutoMapper;
using JenkinsPlayground.Server.Tenants;

namespace JenkinsPlayground.Server.Mappings;

public class Mappings : Profile
{
    public Mappings()
    {
        CreateMap<TenantAddRequest, Tenant>();
        CreateMap<Tenant, TenantResponse>();
    }
}