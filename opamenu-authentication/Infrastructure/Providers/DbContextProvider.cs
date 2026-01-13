using Authenticator.API.Core.Application.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using Authenticator.API.Infrastructure.Data;
using Authenticator.API.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Authenticator.API.Infrastructure.Providers;

/// <summary>
/// Provedor de contexto de banco de dados que determina qual contexto usar baseado no tipo da entidade
/// </summary>
public class DbContextProvider : IDbContextProvider
{
    private readonly AccessControlDbContext _accessControlContext;
    private readonly MultiTenantDbContext _multiTenantContext;

    public DbContextProvider(
        AccessControlDbContext accessControlContext,
        MultiTenantDbContext multiTenantContext)
    {
        _accessControlContext = accessControlContext;
        _multiTenantContext = multiTenantContext;
    }

    /// <summary>
    /// ObtÃ©m o contexto apropriado baseado no tipo da entidade
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <returns>Contexto de banco de dados apropriado</returns>
    public DbContext GetContext<T>() where T : class
    {
        var entityType = typeof(T);

        // Entidades do AccessControl
        if (IsAccessControlEntity(entityType))
        {
            return _accessControlContext;
        }

        // Entidades do MultiTenant
        if (IsMultiTenantEntity(entityType))
        {
            return _multiTenantContext;
        }

        throw new InvalidOperationException($"NÃ£o foi possÃ­vel determinar o contexto para a entidade do tipo {entityType.Name}");
    }

    /// <summary>
    /// ObtÃ©m o DbSet apropriado para o tipo da entidade
    /// </summary>
    /// <typeparam name="T">Tipo da entidade</typeparam>
    /// <returns>DbSet da entidade</returns>
    public DbSet<T> GetDbSet<T>() where T : class
    {
        var context = GetContext<T>();
        return context.Set<T>();
    }

    /// <summary>
    /// Verifica se a entidade pertence ao contexto AccessControl
    /// </summary>
    /// <param name="entityType">Tipo da entidade</param>
    /// <returns>True se pertence ao AccessControl</returns>
    private static bool IsAccessControlEntity(Type entityType)
    {
        return entityType == typeof(UserAccountEntity) ||
               entityType == typeof(AccessGroupEntity) ||
               entityType == typeof(GroupTypeEntity) ||
               entityType == typeof(AccountAccessGroupEntity) ||
               entityType == typeof(RoleTypeEntity) ||
               entityType == typeof(RoleEntity) ||
               entityType == typeof(RoleAccessGroupEntity) ||
               entityType == typeof(RolePermissionEntity) ||
               entityType == typeof(PermissionEntity) ||
               entityType == typeof(OperationEntity) ||
               entityType == typeof(PermissionOperationEntity) ||
               entityType == typeof(ApplicationEntity) ||
               entityType == typeof(ModuleEntity);
    }

    /// <summary>
    /// Verifica se a entidade pertence ao contexto MultiTenant
    /// </summary>
    /// <param name="entityType">Tipo da entidade</param>
    /// <returns>True se pertence ao MultiTenant</returns>
    private static bool IsMultiTenantEntity(Type entityType)
    {
        return entityType == typeof(TenantEntity) ||
               entityType == typeof(TenantBusinessEntity) ||
               entityType == typeof(TenantProductEntity) ||
               entityType == typeof(PlanEntity) ||
               entityType == typeof(SubscriptionEntity);
    }
}

