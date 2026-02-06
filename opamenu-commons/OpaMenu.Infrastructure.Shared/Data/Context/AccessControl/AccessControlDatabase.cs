using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using OpaMenu.Infrastructure.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Data.Context.AccessControl
{

    /// <summary>
    /// Contexto do banco de dados de controle de acesso
    /// Conecta ao banco access_control_db
    /// </summary>
    public class AccessControlDbContext(DbContextOptions<AccessControlDbContext> options, ITenantContext? tenantContext = null) : DbContext(options)
    {
        private readonly ITenantContext _tenantContext = tenantContext ?? new DefaultTenantContext();

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
                entity.Property(e => e.Status).HasColumnName("status");
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
                entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");

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

                entity.HasOne(d => d.Application)
                    .WithMany(p => p.Modules)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<PermissionEntity>(entity =>
            {
                entity.ToTable("permission");
                entity.HasKey(e => e.Id);
                // Índices ajustados: relação Permission↔Role agora é exclusiva via RolePermission (N:N)

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.ModuleId).HasColumnName("module_id");
                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

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

            modelBuilder.Entity<PermissionOperationEntity>().HasQueryFilter(e =>
                !_tenantContext.HasTenant || e.Permission.TenantId == _tenantContext.TenantId);

            modelBuilder.ApplyUtcDateTimeConvention();
            modelBuilder.AccessControlSeed();
        }
    }

    internal static class AccessControlModelBuilderExtensions
    {
        public static void ApplyUtcDateTimeConvention(this ModelBuilder modelBuilder)
        {
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }

                    if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }
        }
    }
}

