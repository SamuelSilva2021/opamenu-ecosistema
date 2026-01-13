using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccountAccessGroups;
using Authenticator.API.Core.Application.Interfaces.AccessControl.RoleAccessGroups;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Roles;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using Authenticator.API.Core.Domain.Api;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;
using AutoMapper;
using System.Text.RegularExpressions;

namespace Authenticator.API.Core.Application.Implementation.MultiTenant
{
    /// <summary>
    /// ServiÃ§o para gerenciamento de tenants (empresas)
    /// </summary>
    /// <param name="tenantRepository"></param>
    /// <param name="mapper"></param>
    /// <param name="logger"></param>
    /// <param name="userAccountsRepository"></param>
    /// <param name="jwtTokenService"></param>
    public class TenantService(
        ITenantRepository tenantRepository,
        IMapper mapper,
        ILogger<TenantService> logger,
        IUserAccountsRepository userAccountsRepository,
        IJwtTokenService jwtTokenService,
        ITenantBusinessRepository tenantBusinessRepository,
        ISubscriptionRepository subscriptionRepository,
        IPlanRepository planRepository,
        ITenantProductRepository tenantProductRepository,
        IAccessGroupRepository accessGroupRepository,
        IAccountAccessGroupRepository accountAccessGroupRepository,
        IRoleRepository roleRepository,
        IRoleAccessGroupRepository roleAccessGroupRepository,
        IGroupTypeRepository groupTypeRepository
        ) : ITenantService
    {
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<TenantService> _logger = logger;
        private readonly IUserAccountsRepository _userAccountsRepository = userAccountsRepository;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
        private readonly ITenantBusinessRepository _tenantBusinessRepository = tenantBusinessRepository;
        private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
        private readonly IPlanRepository _planRepository = planRepository;
        private readonly ITenantProductRepository _tenantProductRepository = tenantProductRepository;
        private readonly IAccessGroupRepository _accessGroupRepository = accessGroupRepository;
        private readonly IAccountAccessGroupRepository _accountAccessGroupRepository = accountAccessGroupRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IRoleAccessGroupRepository _roleAccessGroupRepository = roleAccessGroupRepository;
        private readonly IGroupTypeRepository _groupTypeRepository = groupTypeRepository;

        /// <summary>
        /// Adiciona um novo tenant e cria o usuário administrador associado
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        public async Task<ResponseDTO<RegisterTenantResponseDTO>> AddTenantAsync(CreateTenantDTO tenant)
        {
            try
            {
                var existingUser = await _userAccountsRepository.GetByEmailAsync(tenant.Email);
                if (existingUser != null)
                    return ResponseBuilder<RegisterTenantResponseDTO>
                        .Fail(new ErrorDTO { Message = "Email já está em uso." }).WithCode(400).Build();

                var existingDocument = await _tenantRepository.GetByDocumentAsync(tenant.Document!);
                if (existingDocument != null)
                    return ResponseBuilder<RegisterTenantResponseDTO>
                        .Fail(new ErrorDTO { Message = "CNPJ/CPF já está em uso." }).WithCode(400).Build();

                
                var tenantEntity = _mapper.Map<TenantEntity>(tenant);
                tenantEntity.Slug = await GenerateUniqueSlugAsync(tenant.CompanyName);
                tenantEntity.Status = ETenantStatus.Pendente;

                var createdTenant = await _tenantRepository.AddAsync(tenantEntity);

                var tenantBusinessEntity = new TenantBusinessEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = createdTenant.Id,
                };

                await _tenantBusinessRepository.AddAsync(tenantBusinessEntity);

                var tenantDTO = _mapper.Map<TenantDTO>(createdTenant);
                _logger.LogInformation("Tenant criado com sucesso: {TenantId}", createdTenant.Id);

                // 2. Criar o usuário Administrador
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(tenant.Password);
                var userName = await GenerateUniqueUsernameAsync(tenant.Email);

                var adminUser = new UserAccountEntity
                {
                    Id = Guid.NewGuid(),
                    TenantId = createdTenant.Id,
                    Username = await GenerateUniqueUsernameAsync(tenant.Email),
                    Email = tenant.Email.ToLower().Trim(),
                    PasswordHash = passwordHash,
                    FirstName = tenant.FirstName.Trim(),
                    LastName = tenant.LastName.Trim(),
                    PhoneNumber = tenant.UserPhone?.Trim(),
                    Status = EUserAccountStatus.Inativo,
                    IsEmailVerified = false
                };
                await _userAccountsRepository.AddAsync(adminUser);
                _logger.LogInformation("usuário administrador criado com sucesso: {UserId}", adminUser.Id);

                // 3. Configurar PermissÃµes (Roles e AccessGroups)
                try 
                {
                    // Buscar GroupType "Tenant"
                    var tenantGroupTypes = await _groupTypeRepository.FindAsync(gt => gt.Code == "TENANT");
                    var tenantGroupType = tenantGroupTypes.FirstOrDefault();
                    var groupTypeId = tenantGroupType?.Id ?? Guid.Parse("00000000-0000-0000-0000-000000000002");

                    // Criar Grupo "Administradores"
                    var adminGroup = new AccessGroupEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Administradores",
                        Description = "Grupo de administradores do tenant",
                        TenantId = createdTenant.Id,
                        GroupTypeId = groupTypeId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _accessGroupRepository.AddAsync(adminGroup);

                    var adminRole = new RoleEntity
                    {
                        Id = Guid.NewGuid(),
                        Name = "Admin",
                        Code = "ADMIN",
                        Description = "Administrador do Tenant",
                        TenantId = createdTenant.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _roleRepository.AddAsync(adminRole);

                    // Vincular Role ao Grupo
                    var roleGroup = new RoleAccessGroupEntity
                    {
                        Id = Guid.NewGuid(),
                        RoleId = adminRole.Id,
                        AccessGroupId = adminGroup.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _roleAccessGroupRepository.AddAsync(roleGroup);

                    // Vincular usuário ao Grupo
                    var userGroup = new AccountAccessGroupEntity
                    {
                        Id = Guid.NewGuid(),
                        UserAccountId = adminUser.Id,
                        AccessGroupId = adminGroup.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        GrantedBy = adminUser.Id 
                    };
                    await _accountAccessGroupRepository.AddAsync(userGroup);

                    _logger.LogInformation("PermissÃµes de administrador configuradas para o usuário: {UserId}", adminUser.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao configurar permissÃµes iniciais para o tenant {TenantId}", createdTenant.Id);
                    //Fazer rollback das operaÃ§Ãµes anteriores
                }

                var accessToken = _jwtTokenService.GenerateAccessToken(adminUser, createdTenant, new List<string> { "Admin" });
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                var expiresIn = _jwtTokenService.GetTokenExpirationTime();

                if (!string.IsNullOrWhiteSpace(tenant.LeadSource))
                {
                    _logger.LogInformation("Lead registrado - Fonte: {LeadSource}, UTM: {UtmSource}/{UtmCampaign}/{UtmMedium}",
                        tenant.LeadSource, tenant.UtmSource, tenant.UtmCampaign, tenant.UtmMedium);
                }

                var dto = new RegisterTenantResponseDTO
                {
                    TenantId = createdTenant.Id,
                    UserId = adminUser.Id,
                    CompanyName = createdTenant.Name,
                    Slug = createdTenant.Slug,
                    Email = adminUser.Email,
                    FullName = $"{adminUser.FirstName} {adminUser.LastName}",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresIn = expiresIn,
                    CreatedAt = createdTenant.CreatedAt,
                    Message = "Empresa e usuário administrador criados com sucesso!"
                };

                return ResponseBuilder<RegisterTenantResponseDTO>.Ok(dto).WithCode(201).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar novo tenant: {Message}", ex.Message);
                return ResponseBuilder<RegisterTenantResponseDTO>
                    .Fail(new ErrorDTO { Message = ex.Message }).WithException(ex).WithCode(500).Build();
            }
        }

        /// <summary>
        /// Gera um nome de usuário Ãºnico baseado no email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<string> GenerateUniqueUsernameAsync(string email)
        {
            var baseUsername = email.Split('@')[0].ToLower();
            var username = baseUsername;
            var counter = 1;

            while (await _userAccountsRepository.AnyAsync(u => u.Username == username))
            {
                username = $"{baseUsername}{counter}";
                counter++;
            }

            return username;
        }

        /// <summary>
        /// Gera um slug Ãºnico para o tenant baseado no nome da empresa
        /// </summary>
        private async Task<string> GenerateUniqueSlugAsync(string companyName)
        {
            // Remover caracteres especiais e normalizar
            var slug = Regex.Replace(companyName.ToLower(), @"[^a-z0-9\s-]", "")
                           .Trim()
                           .Replace(' ', '_')
                           .Substring(0, Math.Min(companyName.Length, 50));

            var originalSlug = slug;
            var counter = 1;

            // Verificar se o slug já existe
            while (await _tenantRepository.ExistingSlug(slug))
            {
                slug = $"{originalSlug}_{counter}";
                counter++;
            }

            return slug;
        }
    }
}



