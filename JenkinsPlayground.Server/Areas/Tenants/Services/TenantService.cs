using AutoMapper;
using JenkinsPlayground.Server.Data;
using JenkinsPlayground.Server.Helpers;
using Microsoft.EntityFrameworkCore;

namespace JenkinsPlayground.Server.Tenants;

public class TenantService
{
    private readonly JenkinsPlaygroundContext _context;
    private readonly IMapper _mapper;

    public TenantService(JenkinsPlaygroundContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    private readonly string _selectSql =
        $@"
             SELECT DISTINCT ON (t.id)
                 t.*
             FROM
                 tenant t
             WHERE
                 (@id is null OR t.id = @id)";
    
    private object getSelectSqlParams(Guid? id = null)
    {
        return new { id };
    }
    
    public async Task<Tenant> GetTenantById(Guid id)
    {
        return await _context.Tenants.SingleOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException($"No tenant found for id: {id}");
    }

    public async Task<List<Tenant>> GetAllTenants()
    {
        return await _context.Tenants.ToListAsync() ?? throw new NotFoundException("No tenant found");
    }

    public async Task<List<TenantResponse>> GetTenantsApi()
    {
        var tenants = await GetAllTenants();
        var result = _mapper.Map<List<TenantResponse>>(tenants);
        return result;
    }

    public async Task<TenantResponse> GetTenantApi(Guid id)
    {
        var tenant = await GetTenantById(id);
        var result = _mapper.Map<TenantResponse>(tenant);
        return result;
    }

    public async Task<Tenant> AddTenant(Tenant tenant)
    {
        var result = _context.Tenants.Add(tenant).Entity;
        await _context.SaveChangesAsync();
        return result;
    }
}