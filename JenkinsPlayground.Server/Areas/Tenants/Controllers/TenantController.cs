using AutoMapper;
using JenkinsPlayground.Server.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace JenkinsPlayground.Server.Tenants;

[ApiController]
[Route("/api/v1/tenants")]
public class TenantController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly TenantService _tenantService;

    public TenantController(
        TenantService tenantService,
        IMapper mapper
    )
    {
        _tenantService = tenantService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<TenantResponse>>> GetAll(
    )
    {
        var tenants = await _tenantService.GetTenantsApi();
        return Ok(tenants);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TenantResponse>> Get(Guid id)
    {
        var tenant = await _tenantService.GetTenantApi(id);

        return Ok(tenant);
    }
    
    [HttpPost]
    public async Task<ActionResult<TenantResponse>> Add([FromBody] TenantAddRequest model)
    {
        var tenant = _mapper.Map<Tenant>(model);
        tenant = await _tenantService.AddTenant(tenant);
        var result = await _tenantService.GetTenantApi(tenant.Id);

        return Created("", result);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<TenantResponse>> Update([FromBody] TenantUpdateRequest model, Guid id)
    {
        var tenant = await _tenantService.GetTenantById(id);
        _mapper.Map(model, tenant);
        tenant = await _tenantService.UpdateTenant(tenant);
        var result = await _tenantService.GetTenantApi(tenant.Id);

        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<TenantResponse>> Delete(Guid id)
    {
        var tenant = await _tenantService.GetTenantById(id);
        var result = await _tenantService.GetTenantApi(tenant.Id);
        await _tenantService.RemoveTenant(tenant);

        return Ok(result);
    }
}