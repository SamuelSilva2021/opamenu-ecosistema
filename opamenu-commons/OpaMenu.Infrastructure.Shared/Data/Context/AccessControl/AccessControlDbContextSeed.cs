using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using System.Security.Cryptography;
using System.Text;

namespace OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;

public static class AccessControlDbContextSeed
{
    private static readonly DateTime _seedDate = new(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc);

    public static void AccessControlSeed(this ModelBuilder modelBuilder)
    {
        // 1. Operations
        var opRead = CreateOperation("READ", "Leitura", "Permite visualizar registros");
        var opCreate = CreateOperation("CREATE", "Criação", "Permite criar novos registros");
        var opUpdate = CreateOperation("UPDATE", "Atualização", "Permite atualizar registros existentes");
        var opDelete = CreateOperation("DELETE", "Exclusão", "Permite excluir registros");

        modelBuilder.Entity<OperationEntity>().HasData(opRead, opCreate, opUpdate, opDelete);

        // 2. Group Types
        var gtSystemAdmin = CreateGroupType("SYSTEM_ADMIN", "Administradores do Sistema", "Equipe de analistas do sistema");
        var gtTenantAdmin = CreateGroupType("TENANT_ADMIN", "Administradores do Tenant", "Administradores dos estabelecimentos");

        modelBuilder.Entity<GroupTypeEntity>().HasData(gtSystemAdmin, gtTenantAdmin);

        // 3. Roles
        var roleSuperAdmin = CreateRole("SUPER_ADMIN", "Super Administrador", "Acesso total ao sistema", null); // System level
        var roleAdmin = CreateRole("ADMIN", "Administrador", "Administrador do estabelecimento", null); // Tenant level template

        modelBuilder.Entity<RoleEntity>().HasData(roleSuperAdmin, roleAdmin);

        // 4. Access Groups (Templates)
        var groupSystemAdmin = CreateAccessGroup("System Admins", "Grupo de super administradores", "GRP_SYS_ADMIN", gtSystemAdmin.Id);
        var groupTenantAdmin = CreateAccessGroup("Tenant Admins", "Grupo de administradores de tenant", "GRP_TENANT_ADMIN", gtTenantAdmin.Id);

        modelBuilder.Entity<AccessGroupEntity>().HasData(groupSystemAdmin, groupTenantAdmin);

        // 5. Modules
        var modules = new List<ModuleEntity>
        {
            CreateModule("DASHBOARD", "Dashboard", "Visão geral do sistema", "/dashboard"),
            CreateModule("ADDON_GROUP", "Grupos de Adicionais", "Gerenciamento de grupos de adicionais", "/addon-groups"),
            CreateModule("ADDON", "Adicionais", "Gerenciamento de adicionais", "/addons"),
            CreateModule("PRODUCT", "Produtos", "Gerenciamento de produtos", "/products"),
            CreateModule("SETTINGS", "Configurações", "Configurações do sistema/estabelecimento", "/settings"),
            CreateModule("CATEGORY", "Categorias", "Gerenciamento de categorias", "/categories"),
            CreateModule("ORDER", "Pedidos", "Gerenciamento de pedidos", "/orders"),
            CreateModule("TABLE", "Mesas", "Gerenciamento de mesas", "/tables"),
            CreateModule("CUSTOMER", "Clientes", "Gerenciamento de clientes", "/customers"),
            CreateModule("PAYMENT", "Pagamentos", "Gerenciamento de pagamentos", "/payments"),
            CreateModule("COUPON", "Cupons", "Gerenciamento de cupons", "/coupons"),
            CreateModule("LOYALTY", "Fidelidade", "Programa de fidelidade", "/loyalty")
        };

        modelBuilder.Entity<ModuleEntity>().HasData(modules);

        // 6. Permissions & 7. Links
        var permissions = new List<PermissionEntity>();
        var permissionOperations = new List<PermissionOperationEntity>();
        var rolePermissions = new List<RolePermissionEntity>();

        foreach (var module in modules)
        {
            var permission = CreatePermission(module);
            permissions.Add(permission);

            // Link Permission -> Operations (All 4)
            permissionOperations.Add(CreatePermissionOperation(permission.Id, opRead.Id));
            permissionOperations.Add(CreatePermissionOperation(permission.Id, opCreate.Id));
            permissionOperations.Add(CreatePermissionOperation(permission.Id, opUpdate.Id));
            permissionOperations.Add(CreatePermissionOperation(permission.Id, opDelete.Id));

            // Link Role -> Permission
            // SUPER_ADMIN gets everything
            rolePermissions.Add(CreateRolePermission(roleSuperAdmin.Id, permission.Id));

            // ADMIN gets everything (for now)
            rolePermissions.Add(CreateRolePermission(roleAdmin.Id, permission.Id));
        }

        modelBuilder.Entity<PermissionEntity>().HasData(permissions);
        modelBuilder.Entity<PermissionOperationEntity>().HasData(permissionOperations);
        modelBuilder.Entity<RolePermissionEntity>().HasData(rolePermissions);

        // Link Role -> AccessGroup
        var roleAccessGroups = new List<RoleAccessGroupEntity>
        {
            CreateRoleAccessGroup(roleSuperAdmin.Id, groupSystemAdmin.Id),
            CreateRoleAccessGroup(roleAdmin.Id, groupTenantAdmin.Id)
        };

        modelBuilder.Entity<RoleAccessGroupEntity>().HasData(roleAccessGroups);

        // 8. Initial User (System Admin)
        // Note: Using a fixed hash is required to prevent "PendingModelChangesWarning" because BCrypt generates a new salt every time.
        // Password: "Abc@123"
        var userAdmin = CreateUserAccount("admin", "admin@opamenu.com.br", "System", "Admin", "$2a$11$rR/VYsNgEYRwaJt/bMn2ieq.izZrI8dUMfd4yottdElTWQL/vh7eO");
        modelBuilder.Entity<UserAccountEntity>().HasData(userAdmin);

        // 9. Link User -> AccessGroup (System Admin Group)
        var userAccessGroup = CreateAccountAccessGroup(userAdmin.Id, groupSystemAdmin.Id);
        modelBuilder.Entity<AccountAccessGroupEntity>().HasData(userAccessGroup);
    }


    private static OperationEntity CreateOperation(string value, string name, string description)
    {
        return new OperationEntity
        {
            Id = GenerateId($"OP_{value}"),
            Value = value,
            Name = name,
            Description = description,
            IsActive = true,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static GroupTypeEntity CreateGroupType(string code, string name, string description)
    {
        return new GroupTypeEntity
        {
            Id = GenerateId($"GT_{code}"),
            Code = code,
            Name = name,
            Description = description,
            IsActive = true,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static RoleEntity CreateRole(string code, string name, string description, Guid? tenantId)
    {
        return new RoleEntity
        {
            Id = GenerateId($"ROLE_{code}"),
            Code = code,
            Name = name,
            Description = description,
            TenantId = tenantId,
            IsActive = true,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static AccessGroupEntity CreateAccessGroup(string name, string description, string code, Guid groupTypeId)
    {
        return new AccessGroupEntity
        {
            Id = GenerateId($"AG_{code}"),
            Name = name,
            Description = description,
            Code = code,
            GroupTypeId = groupTypeId,
            IsActive = true,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static ModuleEntity CreateModule(string key, string name, string description, string url)
    {
        return new ModuleEntity
        {
            Id = GenerateId($"MOD_{key}"),
            Key = key,
            Name = name,
            Description = description,
            Url = url,
            IsActive = true,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static PermissionEntity CreatePermission(ModuleEntity module)
    {
        return new PermissionEntity
        {
            Id = GenerateId($"PERM_{module.Key}"),
            ModuleId = module.Id,
            IsActive = true,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static PermissionOperationEntity CreatePermissionOperation(Guid permissionId, Guid operationId)
    {
        return new PermissionOperationEntity
        {
            Id = GenerateId($"PO_{permissionId}_{operationId}"),
            PermissionId = permissionId,
            OperationId = operationId,
            IsActive = true,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static RolePermissionEntity CreateRolePermission(Guid roleId, Guid permissionId)
    {
        return new RolePermissionEntity
        {
            Id = GenerateId($"RP_{roleId}_{permissionId}"),
            RoleId = roleId,
            PermissionId = permissionId,
            IsActive = true,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static RoleAccessGroupEntity CreateRoleAccessGroup(Guid roleId, Guid accessGroupId)
    {
        return new RoleAccessGroupEntity
        {
            Id = GenerateId($"RAG_{roleId}_{accessGroupId}"),
            RoleId = roleId,
            AccessGroupId = accessGroupId,
            IsActive = true,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static Guid GenerateId(string input)
    {
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }
    }

    private static UserAccountEntity CreateUserAccount(string username, string email, string firstName, string lastName, string passwordHash)
    {
        return new UserAccountEntity
        {
            Id = GenerateId($"USER_{username}"),
            Username = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash,
            Status = EUserAccountStatus.Ativo,
            IsEmailVerified = true,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static AccountAccessGroupEntity CreateAccountAccessGroup(Guid userId, Guid accessGroupId)
    {
        return new AccountAccessGroupEntity
        {
            Id = GenerateId($"AAG_{userId}_{accessGroupId}"),
            UserAccountId = userId,
            AccessGroupId = accessGroupId,
            IsActive = true,
            GrantedAt = _seedDate,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }
}
