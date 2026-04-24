using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Application.Services.Interfaces.Auth;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Domain.DTOs.Auth;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.DTOs.MultiTenant;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/tenants")]
[Authorize(Roles = "SUPER_ADMIN")]
public sealed class TenantsController(
    MultiTenantDbContext multiTenantDbContext,
    AccessControlDbContext accessControlDbContext,
    IAuthService authService) : ControllerBase
{
    private readonly MultiTenantDbContext _multiTenantDbContext = multiTenantDbContext;
    private readonly AccessControlDbContext _accessControlDbContext = accessControlDbContext;
    private readonly IAuthService _authService = authService;

    [HttpPost("/api/register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterTenantRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CompanyName) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return BadRequest(StaticResponseBuilder<RegisterTenantResponseDto>.BuildError("Dados inválidos."));
            }

            if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
            {
                return BadRequest(StaticResponseBuilder<RegisterTenantResponseDto>.BuildError("As senhas não coincidem."));
            }

            var email = request.Email.Trim().ToLowerInvariant();
            var emailExists = await _accessControlDbContext.UserAccounts
                .AsNoTracking()
                .AnyAsync(u => u.DeletedAt == null && u.Email.ToLower() == email);

            if (emailExists)
            {
                return BadRequest(StaticResponseBuilder<RegisterTenantResponseDto>.BuildError("Email já cadastrado."));
            }

            var slug = await GenerateUniqueTenantSlugAsync(request.CompanyName);
            var now = DateTime.UtcNow;

            var tenant = new TenantEntity
            {
                Id = Guid.NewGuid(),
                Name = request.CompanyName.Trim(),
                Slug = slug,
                Domain = null,
                Document = request.Document,
                Status = ETenantStatus.Pendente,
                Email = email,
                Phone = null,
                CreatedAt = now,
                UpdatedAt = null
            };

            _multiTenantDbContext.Tenants.Add(tenant);
            await _multiTenantDbContext.SaveChangesAsync();

            var role = await ResolveOrCreateAdminRoleAsync(tenant.Id);

            var usernameBase = email.Split('@')[0];
            var username = await GenerateUniqueUsernameAsync(usernameBase);

            var user = new UserAccountEntity
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PhoneNumber = null,
                Status = EUserAccountStatus.Ativo,
                IsEmailVerified = false,
                RoleId = role?.Id,
                CreatedAt = now,
                UpdatedAt = now
            };

            _accessControlDbContext.UserAccounts.Add(user);
            await _accessControlDbContext.SaveChangesAsync();

            var login = await _authService.LoginAsync(new LoginRequestDto
            {
                UsernameOrEmail = email,
                Password = request.Password
            });

            if (!login.Succeeded || login.Data == null)
            {
                return StatusCode(500, StaticResponseBuilder<RegisterTenantResponseDto>.BuildError("Falha ao gerar token de acesso."));
            }

            var response = new RegisterTenantResponseDto
            {
                TenantId = tenant.Id,
                UserId = user.Id,
                CompanyName = tenant.Name,
                Slug = tenant.Slug,
                Email = user.Email,
                FullName = (user.FirstName + " " + user.LastName).Trim(),
                AccessToken = login.Data.AccessToken,
                RefreshToken = login.Data.RefreshToken,
                ExpiresIn = login.Data.ExpiresIn,
                CreatedAt = now,
                Message = "Tenant cadastrado com sucesso!",
                RedirectToPlanSelection = login?.Data?.RedirectToPlanSelection ?? false
            };

            return Ok(StaticResponseBuilder<RegisterTenantResponseDto>.BuildOk(response));
        }
        catch (Exception ex)
        {
            return StatusCode(500, StaticResponseBuilder<RegisterTenantResponseDto>.BuildErrorResponse(ex));
        }
    }

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

    private async Task<string> GenerateUniqueTenantSlugAsync(string companyName)
    {
        var baseSlug = Slugify(companyName);
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "tenant";
        }

        var slug = baseSlug;
        var exists = await _multiTenantDbContext.Tenants.AsNoTracking().AnyAsync(t => t.Slug == slug);
        if (!exists)
        {
            return slug;
        }

        for (var i = 2; i <= 200; i++)
        {
            slug = $"{baseSlug}-{i}";
            exists = await _multiTenantDbContext.Tenants.AsNoTracking().AnyAsync(t => t.Slug == slug);
            if (!exists)
            {
                return slug;
            }
        }

        return $"{baseSlug}-{Guid.NewGuid():N}";
    }

    private static string Slugify(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (uc == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(ch);
                continue;
            }

            if (ch is ' ' or '-' or '_' or '.')
            {
                sb.Append('-');
            }
        }

        var slug = sb.ToString().Normalize(NormalizationForm.FormC);
        slug = string.Join("-", slug.Split('-', StringSplitOptions.RemoveEmptyEntries));
        return slug;
    }

    private async Task<RoleEntity?> ResolveOrCreateAdminRoleAsync(Guid tenantId)
    {
        var adminRole = await _accessControlDbContext.Roles
            .AsNoTracking()
            .Where(r => r.IsActive && (r.TenantId == null || r.TenantId == tenantId) && r.Code != null && r.Code.ToUpper() == "ADMIN")
            .OrderByDescending(r => r.IsSystem)
            .FirstOrDefaultAsync();

        if (adminRole != null)
        {
            return adminRole;
        }

        var now = DateTime.UtcNow;
        var role = new RoleEntity
        {
            Id = Guid.NewGuid(),
            Name = "Administrador",
            Description = "Role administrativo do tenant",
            Code = "ADMIN",
            TenantId = tenantId,
            ApplicationId = null,
            IsActive = true,
            IsSystem = false,
            CreatedAt = now,
            UpdatedAt = null
        };

        _accessControlDbContext.Roles.Add(role);

        var moduleKeys = await _accessControlDbContext.Modules.AsNoTracking()
            .Where(m => m.IsActive && m.Key != null)
            .Select(m => m.Key!)
            .ToListAsync();

        var defaultActions = new List<string> { "CREATE", "READ", "UPDATE", "DELETE" };
        foreach (var moduleKey in moduleKeys.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            _accessControlDbContext.RolePermissions.Add(new RolePermissionEntity
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                ModuleKey = moduleKey,
                Actions = defaultActions,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = null
            });
        }

        await _accessControlDbContext.SaveChangesAsync();
        return role;
    }

    private async Task<string> GenerateUniqueUsernameAsync(string usernameBase)
    {
        var baseValue = string.IsNullOrWhiteSpace(usernameBase) ? "user" : usernameBase.Trim();
        var candidate = baseValue;

        var exists = await _accessControlDbContext.UserAccounts.AsNoTracking().AnyAsync(u => u.Username == candidate && u.DeletedAt == null);
        if (!exists)
        {
            return candidate;
        }

        for (var i = 0; i < 50; i++)
        {
            candidate = $"{baseValue}{RandomNumberGenerator.GetInt32(1000, 9999)}";
            exists = await _accessControlDbContext.UserAccounts.AsNoTracking().AnyAsync(u => u.Username == candidate && u.DeletedAt == null);
            if (!exists)
            {
                return candidate;
            }
        }

        return $"{baseValue}{Guid.NewGuid():N}";
    }
}
