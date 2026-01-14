using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccountAccessGroups;
using Authenticator.API.Core.Application.Interfaces.AccessControl.RoleAccessGroups;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Roles;
using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using Authenticator.API.Core.Domain.Api;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using Authenticator.API.Core.Domain.MultiTenant.Tenant.DTOs;
using AutoMapper;
using System.Text.RegularExpressions;
using Authenticator.API.Core.Application.Interfaces.AccessControl.UserAccounts;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;

namespace Authenticator.API.Core.Application.Implementation.MultiTenant
{
    /// <summary>
    /// Serviço para gerenciamento de tenants (empresas)
    /// </summary>
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
        IGroupTypeRepository groupTypeRepository,
        IUserAccountService userAccountService
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
        private readonly IUserAccountService _userAccountService = userAccountService;

        public async Task<ResponseDTO<RegisterTenantResponseDTO>> AddTenantAsync(CreateTenantDTO tenant)
        {
            try
            {
                var validationResult = await ValidateNewTenantAsync(tenant);
                if (!validationResult.Success)
                {
                    return ResponseBuilder<RegisterTenantResponseDTO>
                        .Fail(new ErrorDTO { Message = validationResult.Message })
                        .WithCode(400)
                        .Build();
                }

                TenantEntity? createdTenant = null;
                UserAccountDTO? userAdmin = null;

                createdTenant = await CreateTenantEntityAsync(tenant);

                await CreateTenantBusinessAsync(createdTenant.Id);

                _logger.LogInformation("Tenant criado com sucesso: {TenantId}", createdTenant.Id);

                var userAdminResponse = await _userAccountService.CreateUserAccountAdminAsync(createdTenant.Id, tenant);
                if (!userAdminResponse.Succeeded || userAdminResponse.Data == null)
                {
                    throw new InvalidOperationException($"Erro ao criar usuário admin: {userAdminResponse.Errors?.FirstOrDefault()?.Message ?? "Erro desconhecido"}");
                }
                userAdmin = userAdminResponse.Data;

                await ConfigureInitialPermissionsAsync(createdTenant, userAdmin);

                return await GenerateSuccessResponseAsync(createdTenant, userAdmin, tenant);
            }
            catch (Exception ex)
            {
                await TryRollbackTenantCreationAsync(ex, tenant);

                var baseException = ex.GetBaseException();

                _logger.LogError(ex, "Erro ao criar novo tenant: {Message}", baseException.Message);
                return ResponseBuilder<RegisterTenantResponseDTO>
                    .Fail(new ErrorDTO { Message = baseException.Message })
                    .WithException(ex)
                    .WithCode(500)
                    .Build();
            }
        }

        private async Task TryRollbackTenantCreationAsync(Exception originalException, CreateTenantDTO tenant)
        {
            try
            {
                var createdTenant = await _tenantRepository.GetByDocumentAsync(tenant.Document!);
                if (createdTenant != null)
                {
                    var tenantBusiness = await _tenantBusinessRepository.GetByTenantIdAsync(createdTenant.Id);
                    if (tenantBusiness != null)
                    {
                        await _tenantBusinessRepository.DeleteAsync(createdTenant.Id);
                    }

                    await _tenantRepository.DeleteAsync(createdTenant);
                }
            }
            catch (Exception rollbackEx)
            {
                _logger.LogError(rollbackEx, "Erro ao tentar fazer rollback da criação de tenant após falha: {Message}", originalException.Message);
            }
        }

        private async Task<(bool Success, string Message)> ValidateNewTenantAsync(CreateTenantDTO tenant)
        {
            var existingUser = await _userAccountsRepository.GetByEmailAsync(tenant.Email);
            if (existingUser != null)
                return (false, "Email já está em uso.");

            var existingDocument = await _tenantRepository.GetByDocumentAsync(tenant.Document!);
            if (existingDocument != null)
                return (false, "CNPJ/CPF já está em uso.");

            return (true, string.Empty);
        }

        private async Task<TenantEntity> CreateTenantEntityAsync(CreateTenantDTO tenantDto)
        {
            var tenantEntity = _mapper.Map<TenantEntity>(tenantDto);
            tenantEntity.Slug = await GenerateUniqueSlugAsync(tenantDto.CompanyName);
            tenantEntity.Status = ETenantStatus.Pendente;
            
            return await _tenantRepository.AddAsync(tenantEntity);
        }

        private async Task CreateTenantBusinessAsync(Guid tenantId)
        {
            var tenantBusinessEntity = new TenantBusinessEntity
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
            };

            await _tenantBusinessRepository.AddAsync(tenantBusinessEntity);
        }

        private async Task ConfigureInitialPermissionsAsync(TenantEntity createdTenant, UserAccountDTO userAdmin)
        {
            try
            {
                // Buscar GroupType "TENANT"
                var tenantGroupType = await _groupTypeRepository.FirstOrDefaultAsync(gt => gt.Code == "TENANT");
                if (tenantGroupType == null)
                    throw new InvalidOperationException("Tipo de grupo TENANT não encontrado.");

                var adminRole = (await _roleRepository.FindAsync(r => r.Code == "ADMIN")).FirstOrDefault();
                if (adminRole == null)
                    throw new InvalidOperationException("Role ADMIN não encontrada.");

                // Criar Grupo "Administradores para a loja"
                var adminGroup = new AccessGroupEntity
                {
                    Id = Guid.NewGuid(),
                    Name = createdTenant.Slug,
                    Code = "TENANT_ADMINS",
                    Description = "Grupo de administradores do tenant",
                    TenantId = createdTenant.Id,
                    GroupTypeId = tenantGroupType.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _accessGroupRepository.AddAsync(adminGroup);

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
                    UserAccountId = userAdmin.Id,
                    AccessGroupId = adminGroup.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    GrantedBy = userAdmin.Id,
                };
                await _accountAccessGroupRepository.AddAsync(userGroup);

                _logger.LogInformation("Permissões de administrador configuradas para o usuário: {UserId}", userAdmin.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao configurar permissões iniciais para o tenant {TenantId}", createdTenant.Id);
                throw; // Re-throw para acionar o rollback
            }
        }

        private async Task<ResponseDTO<RegisterTenantResponseDTO>> GenerateSuccessResponseAsync(TenantEntity createdTenant, UserAccountDTO userAdmin, CreateTenantDTO tenantDto)
        {
            var accessToken = _jwtTokenService.GenerateAccessToken(userAdmin, createdTenant, new List<string> { "Admin" });
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var expiresIn = _jwtTokenService.GetTokenExpirationTime();

            if (!string.IsNullOrWhiteSpace(tenantDto.LeadSource))
            {
                _logger.LogInformation("Lead registrado - Fonte: {LeadSource}, UTM: {UtmSource}/{UtmCampaign}/{UtmMedium}",
                    tenantDto.LeadSource, tenantDto.UtmSource, tenantDto.UtmCampaign, tenantDto.UtmMedium);
            }

            var dto = new RegisterTenantResponseDTO
            {
                TenantId = createdTenant.Id,
                UserId = userAdmin.Id,
                CompanyName = createdTenant.Name,
                Slug = createdTenant.Slug,
                Email = userAdmin.Email,
                FullName = $"{userAdmin.FirstName} {userAdmin.LastName}",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresIn,
                CreatedAt = createdTenant.CreatedAt,
                Message = "Empresa e usuário administrador criados com sucesso!"
            };

            return ResponseBuilder<RegisterTenantResponseDTO>.Ok(dto).WithCode(201).Build();
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



