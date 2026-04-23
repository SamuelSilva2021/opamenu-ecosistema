using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.DTOs.MultiTenant;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/tenants")]
[Authorize(Roles = "SUPER_ADMIN")]
public sealed class TenantsController(MultiTenantDbContext multiTenantDbContext, AccessControlDbContext accessControlDbContext) : ControllerBase
{
    private readonly MultiTenantDbContext _multiTenantDbContext = multiTenantDbContext;
    private readonly AccessControlDbContext _accessControlDbContext = accessControlDbContext;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TenantSummaryDto>>> GetTenants([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _multiTenantDbContext.Tenants.AsNoTracking().AsQueryable();

        var filterName = Request.Query["filter.name"].ToString();
        var filterSlug = Request.Query["filter.slug"].ToString();
        var filterDomain = Request.Query["filter.domain"].ToString();
        var filterEmail = Request.Query["filter.email"].ToString();
        var filterPhone = Request.Query["filter.phone"].ToString();
        var filterStatus = Request.Query["filter.status"].ToString();

        if (!string.IsNullOrWhiteSpace(filterName))
        {
            query = query.Where(t => t.Name.Contains(filterName));
        }

        if (!string.IsNullOrWhiteSpace(filterSlug))
        {
            query = query.Where(t => t.Slug.Contains(filterSlug));
        }

        if (!string.IsNullOrWhiteSpace(filterDomain))
        {
            query = query.Where(t => t.Domain != null && t.Domain.Contains(filterDomain));
        }

        if (!string.IsNullOrWhiteSpace(filterEmail))
        {
            query = query.Where(t => t.Email != null && t.Email.Contains(filterEmail));
        }

        if (!string.IsNullOrWhiteSpace(filterPhone))
        {
            query = query.Where(t => t.Phone != null && t.Phone.Contains(filterPhone));
        }

        if (!string.IsNullOrWhiteSpace(filterStatus))
        {
            query = query.Where(t => t.Status.ToString() == filterStatus);
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(t => new TenantSummaryDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                Domain = t.Domain,
                Status = t.Status.ToString(),
                Email = t.Email,
                Phone = t.Phone,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ActiveSubscriptionId = t.ActiveSubscriptionId
            })
            .ToListAsync();

        return Ok(new PagedResultDto<TenantSummaryDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenantDto>> GetById([FromRoute] Guid id)
    {
        var tenant = await _multiTenantDbContext.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (tenant == null)
        {
            return NotFound();
        }

        return Ok(MapTenant(tenant));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TenantDto>> Update([FromRoute] Guid id, [FromBody] UpdateTenantRequestDto request)
    {
        var tenant = await _multiTenantDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == id);
        if (tenant == null)
        {
            return NotFound();
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest();
            }
            tenant.Name = request.Name.Trim();
        }

        if (request.Slug != null)
        {
            if (string.IsNullOrWhiteSpace(request.Slug))
            {
                return BadRequest();
            }

            var existsSlug = await _multiTenantDbContext.Tenants.AsNoTracking().AnyAsync(t => t.Id != id && t.Slug == request.Slug);
            if (existsSlug)
            {
                return BadRequest();
            }

            tenant.Slug = request.Slug.Trim();
        }

        if (request.Domain != null)
        {
            tenant.Domain = request.Domain;
        }

        if (request.Status != null)
        {
            if (!Enum.TryParse<ETenantStatus>(request.Status, ignoreCase: true, out var status))
            {
                return BadRequest();
            }
            tenant.Status = status;
        }

        if (request.CnpjCpf != null) tenant.Document = request.CnpjCpf;
        if (request.RazaoSocial != null) tenant.RazaoSocial = request.RazaoSocial;
        if (request.InscricaoEstadual != null) tenant.InscricaoEstadual = request.InscricaoEstadual;
        if (request.InscricaoMunicipal != null) tenant.InscricaoMunicipal = request.InscricaoMunicipal;
        if (request.Phone != null) tenant.Phone = request.Phone;
        if (request.Email != null) tenant.Email = request.Email;
        if (request.Website != null) tenant.Website = request.Website;

        if (request.AddressStreet != null) tenant.AddressStreet = request.AddressStreet;
        if (request.AddressNumber != null) tenant.AddressNumber = request.AddressNumber;
        if (request.AddressComplement != null) tenant.AddressComplement = request.AddressComplement;
        if (request.AddressNeighborhood != null) tenant.AddressNeighborhood = request.AddressNeighborhood;
        if (request.AddressCity != null) tenant.AddressCity = request.AddressCity;
        if (request.AddressState != null) tenant.AddressState = request.AddressState;
        if (request.AddressZipcode != null) tenant.AddressZipcode = request.AddressZipcode;
        if (request.AddressCountry != null) tenant.AddressCountry = request.AddressCountry;

        if (request.BillingStreet != null) tenant.BillingStreet = request.BillingStreet;
        if (request.BillingNumber != null) tenant.BillingNumber = request.BillingNumber;
        if (request.BillingComplement != null) tenant.BillingComplement = request.BillingComplement;
        if (request.BillingNeighborhood != null) tenant.BillingNeighborhood = request.BillingNeighborhood;
        if (request.BillingCity != null) tenant.BillingCity = request.BillingCity;
        if (request.BillingState != null) tenant.BillingState = request.BillingState;
        if (request.BillingZipcode != null) tenant.BillingZipcode = request.BillingZipcode;
        if (request.BillingCountry != null) tenant.BillingCountry = request.BillingCountry;

        if (request.LegalRepresentativeName != null) tenant.LegalRepresentativeName = request.LegalRepresentativeName;
        if (request.LegalRepresentativeCpf != null) tenant.LegalRepresentativeCpf = request.LegalRepresentativeCpf;
        if (request.LegalRepresentativeEmail != null) tenant.LegalRepresentativeEmail = request.LegalRepresentativeEmail;
        if (request.LegalRepresentativePhone != null) tenant.LegalRepresentativePhone = request.LegalRepresentativePhone;

        if (request.ActiveSubscriptionId.HasValue) tenant.ActiveSubscriptionId = request.ActiveSubscriptionId;
        if (request.Settings != null) tenant.Settings = request.Settings;

        tenant.UpdatedAt = DateTime.UtcNow;
        await _multiTenantDbContext.SaveChangesAsync();

        return Ok(MapTenant(tenant));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var tenant = await _multiTenantDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == id);
        if (tenant == null)
        {
            return NoContent();
        }

        tenant.Status = ETenantStatus.Suspenso;
        tenant.UpdatedAt = DateTime.UtcNow;
        await _multiTenantDbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("{tenantId:guid}/modules")]
    public async Task<ActionResult<List<ModuleDto>>> GetTenantModules([FromRoute] Guid tenantId)
    {
        var moduleIds = await _multiTenantDbContext.Set<TenantModuleEntity>()
            .AsNoTracking()
            .Where(tm => tm.TenantId == tenantId && tm.IsEnabled)
            .Select(tm => tm.ModuleId)
            .ToListAsync();

        if (moduleIds.Count == 0)
        {
            return Ok(new List<ModuleDto>());
        }

        var modules = await _accessControlDbContext.Modules.AsNoTracking().Include(m => m.Application)
            .Where(m => moduleIds.Contains(m.Id))
            .Select(m => new ModuleDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Url = m.Url,
                Key = m.Key,
                Code = m.Code,
                ApplicationId = m.ApplicationId,
                ApplicationName = m.Application != null ? m.Application.Name : null,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync();

        return Ok(modules);
    }

    [HttpPost("{tenantId:guid}/modules/{moduleId:guid}")]
    public async Task<IActionResult> AddTenantModule([FromRoute] Guid tenantId, [FromRoute] Guid moduleId)
    {
        var tenantExists = await _multiTenantDbContext.Tenants.AsNoTracking().AnyAsync(t => t.Id == tenantId);
        if (!tenantExists)
        {
            return NotFound();
        }

        var moduleExists = await _accessControlDbContext.Modules.AsNoTracking().AnyAsync(m => m.Id == moduleId);
        if (!moduleExists)
        {
            return BadRequest();
        }

        var existing = await _multiTenantDbContext.Set<TenantModuleEntity>().FirstOrDefaultAsync(tm => tm.TenantId == tenantId && tm.ModuleId == moduleId);
        if (existing == null)
        {
            _multiTenantDbContext.Set<TenantModuleEntity>().Add(new TenantModuleEntity
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ModuleId = moduleId,
                IsEnabled = true,
                Configuration = "{}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            });
        }
        else
        {
            existing.IsEnabled = true;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _multiTenantDbContext.SaveChangesAsync();
        return Ok(true);
    }

    [HttpDelete("{tenantId:guid}/modules/{moduleId:guid}")]
    public async Task<IActionResult> RemoveTenantModule([FromRoute] Guid tenantId, [FromRoute] Guid moduleId)
    {
        var existing = await _multiTenantDbContext.Set<TenantModuleEntity>().FirstOrDefaultAsync(tm => tm.TenantId == tenantId && tm.ModuleId == moduleId);
        if (existing == null)
        {
            return Ok(true);
        }

        existing.IsEnabled = false;
        existing.UpdatedAt = DateTime.UtcNow;
        await _multiTenantDbContext.SaveChangesAsync();
        return Ok(true);
    }

    private static TenantDto MapTenant(TenantEntity tenant)
    {
        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Slug = tenant.Slug,
            Domain = tenant.Domain,
            Status = tenant.Status.ToString(),
            CnpjCpf = tenant.Document,
            RazaoSocial = tenant.RazaoSocial,
            InscricaoEstadual = tenant.InscricaoEstadual,
            InscricaoMunicipal = tenant.InscricaoMunicipal,
            Phone = tenant.Phone,
            Email = tenant.Email,
            Website = tenant.Website,
            AddressStreet = tenant.AddressStreet,
            AddressNumber = tenant.AddressNumber,
            AddressComplement = tenant.AddressComplement,
            AddressNeighborhood = tenant.AddressNeighborhood,
            AddressCity = tenant.AddressCity,
            AddressState = tenant.AddressState,
            AddressZipcode = tenant.AddressZipcode,
            AddressCountry = tenant.AddressCountry,
            BillingStreet = tenant.BillingStreet,
            BillingNumber = tenant.BillingNumber,
            BillingComplement = tenant.BillingComplement,
            BillingNeighborhood = tenant.BillingNeighborhood,
            BillingCity = tenant.BillingCity,
            BillingState = tenant.BillingState,
            BillingZipcode = tenant.BillingZipcode,
            BillingCountry = tenant.BillingCountry,
            LegalRepresentativeName = tenant.LegalRepresentativeName,
            LegalRepresentativeCpf = tenant.LegalRepresentativeCpf,
            LegalRepresentativeEmail = tenant.LegalRepresentativeEmail,
            LegalRepresentativePhone = tenant.LegalRepresentativePhone,
            ActiveSubscriptionId = tenant.ActiveSubscriptionId,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt,
            Settings = tenant.Settings
        };
    }
}

