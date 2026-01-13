using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpaMenu.Infrastructure.Shared.Interfaces;

namespace Authenticator.API.Infrastructure.Data.Context;

/// <summary>
/// Contexto do banco de dados de controle de acesso
/// Conecta ao banco access_control_db
/// </summary>
public class AccessControlDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    // Construtor utilizado pelos testes e cenÃ¡rios sem injeÃ§Ã£o de ITenantContext
    public AccessControlDbContext(DbContextOptions<AccessControlDbContext> options) : base(options)
    {
        _tenantContext = new DefaultTenantContext();
    }

    // Construtor utilizado em execuÃ§Ã£o normal com ITenantContext resolvido pelo DI
    public AccessControlDbContext(DbContextOptions<AccessControlDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<UserAccountEntity> UserAccounts { get; set; }
    public DbSet<AccessGroupEntity> AccessGroups { get; set; }
    public DbSet<GroupTypeEntity> GroupTypes { get; set; }
    public DbSet<AccountAccessGroupEntity> AccountAccessGroups { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<RoleAccessGroupEntity> RoleAccessGroups { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<PermissionOperationEntity> PermissionOperations { get; set; }
    public DbSet<OperationEntity> Operations { get; set; }
    public DbSet<ApplicationEntity> Applications { get; set; }
    public DbSet<ModuleEntity> Modules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<TenantProductEntity>();
        modelBuilder.Ignore<PlanEntity>();
        modelBuilder.Ignore<SubscriptionEntity>();
        modelBuilder.Ignore<TenantBusinessEntity>();

        modelBuilder.Entity<TenantEntity>(entity =>
        {
            entity.ToTable("tenants", t => t.ExcludeFromMigrations());
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.Domain).HasColumnName("domain");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.Property(e => e.Settings)
                .HasColumnName("settings")
                .HasColumnType("jsonb");

            entity.Ignore(e => e.UserAccounts);
            entity.Ignore(e => e.AccessGroups);
            entity.Ignore(e => e.Roles);
            entity.Ignore(e => e.Permissions);
            entity.Ignore(e => e.Subscriptions);
            entity.Ignore(e => e.ActiveSubscription);
            entity.Ignore(e => e.BusinessInfo);
        });

        modelBuilder.Entity<UserAccountEntity>(entity =>
        {
            entity.ToTable("user_account");
            entity.HasKey(e => e.Id);
            entity.Property(u => u.Id).HasColumnName("id").IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.TenantId);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").HasMaxLength(20);
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified").HasDefaultValue(false);
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.PasswordResetToken).HasColumnName("password_reset_token").HasMaxLength(500);
            entity.Property(e => e.PasswordResetExpiresAt).HasColumnName("password_reset_expires_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at").HasColumnType("timestamp without time zone");

            entity.HasMany(u => u.AccountAccessGroups)
                .WithOne(aag => aag.UserAccount)
                .HasForeignKey(aag => aag.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AccessGroupEntity>(entity =>
        {
            entity.ToTable("access_group");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.GroupTypeId).HasColumnName("group_type_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasOne(e => e.GroupType)
                .WithMany(gt => gt.AccessGroups)
                .HasForeignKey(e => e.GroupTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(g => g.AccountAccessGroups)
                .WithOne(ag => ag.AccessGroup)
                .HasForeignKey(ag => ag.AccessGroupId);

            entity.HasMany(g => g.RoleAccessGroups)
                .WithOne(gr => gr.AccessGroup)
                .HasForeignKey(gr => gr.AccessGroupId);
        });

        modelBuilder.Entity<GroupTypeEntity>(entity =>
        {
            entity.ToTable("group_type");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Id).HasColumnName("id").IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
            entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasIndex(gt => gt.Code).IsUnique();

            entity.HasMany(gt => gt.AccessGroups)
                .WithOne(ag => ag.GroupType)
                .HasForeignKey(ag => ag.GroupTypeId);
        });

        modelBuilder.Entity<AccountAccessGroupEntity>(entity =>
        {
            entity.ToTable("account_access_group");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserAccountId, e.AccessGroupId }).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserAccountId).HasColumnName("user_account_id");
            entity.Property(e => e.AccessGroupId).HasColumnName("access_group_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
            entity.Property(e => e.GrantedBy).HasColumnName("granted_by");
            entity.Property(e => e.GrantedAt).HasColumnName("granted_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

            entity.HasOne(x => x.UserAccount)
                .WithMany(x => x.AccountAccessGroups)
                .HasForeignKey(x => x.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.AccessGroup)
                .WithMany(p => p.AccountAccessGroups)
                .HasForeignKey(d => d.AccessGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RoleEntity>(entity =>
        {
            entity.ToTable("role");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(50);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasMany(r => r.RoleAccessGroups)
                .WithOne(gr => gr.Role)
                .HasForeignKey(gr => gr.RoleId);

            entity.HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId);
        });

        modelBuilder.Entity<RoleAccessGroupEntity>(entity =>
        {
            entity.ToTable("role_access_group");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.AccessGroupId, e.RoleId }).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessGroupId).HasColumnName("access_group_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.RoleAccessGroups)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.AccessGroup)
                .WithMany(p => p.RoleAccessGroups)
                .HasForeignKey(d => d.AccessGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolePermissionEntity>(entity =>
        {
            entity.ToTable("role_permission");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<ApplicationEntity>(entity =>
        {
            entity.ToTable("application");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.SecretKey).HasColumnName("secret_key").HasMaxLength(255);
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.AuxiliarSchema).HasColumnName("auxiliar_schema");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Visible).HasColumnName("visible");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<ModuleEntity>(entity =>
        {
            entity.ToTable("module");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Key);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Key).HasColumnName("key").HasMaxLength(100);
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Application)
                .WithMany(p => p.Modules)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PermissionEntity>(entity =>
        {
            entity.ToTable("permission");
            entity.HasKey(e => e.Id);
            // Ãndices ajustados: relaÃ§Ã£o Permissionâ†”Role agora Ã© exclusiva via RolePermission (N:N)

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
            
            entity.HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionId);
        });

        modelBuilder.Entity<OperationEntity>(entity =>
        {
            entity.ToTable("operation");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Value).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Value).HasColumnName("value");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<PermissionOperationEntity>(entity =>
        {
            entity.ToTable("permission_operation");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.PermissionId, e.OperationId }).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.OperationId).HasColumnName("operation_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Permission)
                .WithMany(p => p.PermissionOperations)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Operation)
                .WithMany(p => p.PermissionOperations)
                .HasForeignKey(d => d.OperationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);

        // Filtros globais por tenant
        modelBuilder.Entity<UserAccountEntity>().HasQueryFilter(e => !_tenantContext.HasTenant || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<AccessGroupEntity>().HasQueryFilter(e => !_tenantContext.HasTenant || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<RoleEntity>().HasQueryFilter(e => !_tenantContext.HasTenant || e.TenantId == _tenantContext.TenantId);
        modelBuilder.Entity<PermissionEntity>().HasQueryFilter(e => !_tenantContext.HasTenant || e.TenantId == _tenantContext.TenantId);

        // Adicione filtros para as entidades de relacionamento baseados em suas entidades pai
        modelBuilder.Entity<AccountAccessGroupEntity>().HasQueryFilter(e =>
            !_tenantContext.HasTenant || e.AccessGroup.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<RoleAccessGroupEntity>().HasQueryFilter(e =>
            !_tenantContext.HasTenant || e.AccessGroup.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<RolePermissionEntity>().HasQueryFilter(e =>
            !_tenantContext.HasTenant || e.Permission.TenantId == _tenantContext.TenantId);

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

        // Seed GroupTypes
        modelBuilder.Entity<GroupTypeEntity>().HasData(
            new GroupTypeEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Sistema",
                Code = "SYSTEM",
                Description = "Grupos de acesso do sistema",
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new GroupTypeEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Tenant",
                Code = "TENANT",
                Description = "Grupos de acesso do tenant",
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed Roles
        modelBuilder.Entity<RoleEntity>().HasData(
            new RoleEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000010"),
                Name = "Super Admin",
                Code = "SUPER_ADMIN",
                Description = "Administrador do Sistema",
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new RoleEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000020"),
                Name = "Admin",
                Code = "ADMIN",
                Description = "Administrador do Tenant",
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );
    }
}


