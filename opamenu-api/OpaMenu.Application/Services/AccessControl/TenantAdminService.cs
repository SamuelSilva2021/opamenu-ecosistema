using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Application.Services.Interfaces.Auth;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.DTOs.Auth;
using OpaMenu.Domain.DTOs.MultiTenant;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace OpaMenu.Application.Services.AccessControl;

public sealed class TenantAdminService(
    ITenantRepository tenantRepository,
    ITenantModuleRepository tenantModuleRepository,
    IUserAccountRepository userAccountRepository,
    IRoleRepository roleRepository,
    IRolePermissionRepository rolePermissionRepository,
    IAccessControlModuleRepository accessControlModuleRepository,
    IModuleRepository moduleRepository,
    IAuthService authService,
    OpaMenu.Infrastructure.Shared.Interfaces.ITenantContext tenantContext) : ITenantAdminService
{
    private readonly ITenantRepository _tenantRepository = tenantRepository;
    private readonly ITenantModuleRepository _tenantModuleRepository = tenantModuleRepository;
    private readonly IUserAccountRepository _userAccountRepository = userAccountRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository = rolePermissionRepository;
    private readonly IAccessControlModuleRepository _accessControlModuleRepository = accessControlModuleRepository;
    private readonly IModuleRepository _moduleRepository = moduleRepository;
    private readonly IAuthService _authService = authService;
    private readonly OpaMenu.Infrastructure.Shared.Interfaces.ITenantContext _tenantContext = tenantContext;

    public async Task<(ResponseDTO<RegisterTenantResponseDto> Body, int StatusCode)> RegisterAsync(RegisterTenantRequestDto request)
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
                return (StaticResponseBuilder<RegisterTenantResponseDto>.BuildError("Dados inválidos."), 400);
            }

            if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
            {
                return (StaticResponseBuilder<RegisterTenantResponseDto>.BuildError("As senhas não coincidem."), 400);
            }

            var email = request.Email.Trim().ToLowerInvariant();
            var emailExists = await _userAccountRepository.EmailExistsAsync(email);
            if (emailExists)
            {
                return (StaticResponseBuilder<RegisterTenantResponseDto>.BuildError("Email já cadastrado."), 400);
            }

            if (!string.IsNullOrWhiteSpace(request.Document))
            {
                var documentExists = await _tenantRepository.DocumentExistsAsync(request.Document);
                if (documentExists)
                {
                    return (StaticResponseBuilder<RegisterTenantResponseDto>.BuildError("Documento (CNPJ/CPF) já cadastrado em outra loja."), 400);
                }
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

            // Hack de Segurança: Sobrescrevemos o contexto do tenant com o recém-gerado ID.
            // Isso evita a "System.InvalidOperationException: Tentativa de criar entidade com TenantId diferente do contexto atual"
            // causada por tokens velhos residuais enviados indevidamente pelo Frontend na rota de [AllowAnonymous] Register.
            _tenantContext.SetTenant(tenant.Id, tenant.Slug, tenant.Domain);

            await _tenantRepository.AddAsync(tenant);
            await _tenantRepository.SaveChangesAsync();

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

            await _userAccountRepository.AddAsync(user);
            await _userAccountRepository.SaveChangesAsync();

            var login = await _authService.LoginAsync(new LoginRequestDto
            {
                UsernameOrEmail = email,
                Password = request.Password
            });

            if (!login.Succeeded || login.Data == null)
            {
                return (StaticResponseBuilder<RegisterTenantResponseDto>.BuildError("Falha ao gerar token de acesso."), 500);
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

            return (StaticResponseBuilder<RegisterTenantResponseDto>.BuildOk(response), 200);
        }
        catch (Exception ex)
        {
            return (StaticResponseBuilder<RegisterTenantResponseDto>.BuildErrorResponse(ex), 500);
        }
    }

    public async Task<PagedResultDto<TenantSummaryDto>> GetTenantsAsync(
        int page,
        int limit,
        string? filterName,
        string? filterSlug,
        string? filterDomain,
        string? filterEmail,
        string? filterPhone,
        string? filterStatus)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var (items, total) = await _tenantRepository.GetPagedAsync(page, limit, filterName, filterSlug, filterDomain, filterEmail, filterPhone, filterStatus);
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        return new PagedResultDto<TenantSummaryDto>
        {
            Items = items.Select(t => new TenantSummaryDto
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
            }).ToList(),
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        };
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id);
        return tenant == null ? null : MapTenant(tenant);
    }

    public async Task<(TenantDto? Tenant, bool NotFound, bool BadRequest)> UpdateAsync(Guid id, UpdateTenantRequestDto request)
    {
        var tenant = await _tenantRepository.GetByIdTrackedAsync(id);
        if (tenant == null)
        {
            return (null, true, false);
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return (null, false, true);
            }
            tenant.Name = request.Name.Trim();
        }

        if (request.Slug != null)
        {
            if (string.IsNullOrWhiteSpace(request.Slug))
            {
                return (null, false, true);
            }

            var slug = request.Slug.Trim();
            var existsSlug = await _tenantRepository.SlugExistsAsync(slug, excludeId: id);
            if (existsSlug)
            {
                return (null, false, true);
            }

            tenant.Slug = slug;
        }

        if (request.Domain != null)
        {
            tenant.Domain = request.Domain;
        }

        if (request.Status != null)
        {
            if (!Enum.TryParse<ETenantStatus>(request.Status, ignoreCase: true, out var status))
            {
                return (null, false, true);
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
        await _tenantRepository.SaveChangesAsync();

        return (MapTenant(tenant), false, false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var tenant = await _tenantRepository.GetByIdTrackedAsync(id);
        if (tenant == null)
        {
            return;
        }

        tenant.Status = ETenantStatus.Suspenso;
        tenant.UpdatedAt = DateTime.UtcNow;
        await _tenantRepository.SaveChangesAsync();
    }

    public async Task<List<ModuleDto>> GetTenantModulesAsync(Guid tenantId)
    {
        var moduleIds = await _tenantModuleRepository.GetEnabledModuleIdsAsync(tenantId);
        if (moduleIds.Count == 0)
        {
            return [];
        }

        var modules = await _moduleRepository.GetByIdsWithApplicationAsync(moduleIds);
        return modules.Select(m => new ModuleDto
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
        }).ToList();
    }

    public async Task<(bool Success, bool NotFound, bool BadRequest)> AddTenantModuleAsync(Guid tenantId, Guid moduleId)
    {
        var tenantExists = await _tenantRepository.ExistsAsync(tenantId);
        if (!tenantExists)
        {
            return (false, true, false);
        }

        var moduleExists = await _moduleRepository.GetByIdAsync(moduleId);
        if (moduleExists == null)
        {
            return (false, false, true);
        }

        var existing = await _tenantModuleRepository.GetByTenantAndModuleTrackedAsync(tenantId, moduleId);
        if (existing == null)
        {
            await _tenantModuleRepository.AddAsync(new TenantModuleEntity
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

        await _tenantModuleRepository.SaveChangesAsync();
        return (true, false, false);
    }

    public async Task<bool> RemoveTenantModuleAsync(Guid tenantId, Guid moduleId)
    {
        var existing = await _tenantModuleRepository.GetByTenantAndModuleTrackedAsync(tenantId, moduleId);
        if (existing == null)
        {
            return true;
        }

        existing.IsEnabled = false;
        existing.UpdatedAt = DateTime.UtcNow;
        await _tenantModuleRepository.SaveChangesAsync();
        return true;
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
        var exists = await _tenantRepository.SlugExistsAsync(slug);
        if (!exists)
        {
            return slug;
        }

        for (var i = 2; i <= 200; i++)
        {
            slug = $"{baseSlug}-{i}";
            exists = await _tenantRepository.SlugExistsAsync(slug);
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
        var adminRole = await _roleRepository.GetActiveAdminRoleForTenantAsync(tenantId);
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

        await _roleRepository.AddAsync(role);

        var modules = await _accessControlModuleRepository.GetActiveModulesWithKeyAsync();
        var moduleKeys = modules.Select(m => m.Key!).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        var defaultActions = new List<string> { "CREATE", "READ", "UPDATE", "DELETE" };
        var permissions = moduleKeys.Select(moduleKey => new RolePermissionEntity
        {
            Id = Guid.NewGuid(),
            RoleId = role.Id,
            ModuleKey = moduleKey,
            Actions = defaultActions,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = null
        }).ToList();

        await _rolePermissionRepository.AddRangeAsync(permissions);
        await _rolePermissionRepository.SaveChangesAsync();
        return role;
    }

    private async Task<string> GenerateUniqueUsernameAsync(string usernameBase)
    {
        var baseValue = string.IsNullOrWhiteSpace(usernameBase) ? "user" : usernameBase.Trim();
        var candidate = baseValue;

        var exists = await _userAccountRepository.UsernameExistsAsync(candidate);
        if (!exists)
        {
            return candidate;
        }

        for (var i = 0; i < 50; i++)
        {
            candidate = $"{baseValue}{RandomNumberGenerator.GetInt32(1000, 9999)}";
            exists = await _userAccountRepository.UsernameExistsAsync(candidate);
            if (!exists)
            {
                return candidate;
            }
        }

        return $"{baseValue}{Guid.NewGuid():N}";
    }
}

