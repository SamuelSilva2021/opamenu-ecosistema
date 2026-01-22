using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantModule;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;
using OpaMenu.Infrastructure.Shared.Enums.MultiTenant;
using OpaMenu.Infrastructure.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Shared.Data.Context.MultTenant
{
    /// <summary>
    /// Contexto do banco de dados multi-tenant
    /// Conecta ao banco multi_tenant_db
    /// </summary>
    public class MultiTenantDbContext(DbContextOptions<MultiTenantDbContext> options) : DbContext(options)
    {
        public DbSet<TenantEntity> Tenants { get; set; }
        public DbSet<TenantBusinessEntity> TenantBusinessInfos { get; set; }
        public DbSet<TenantProductEntity> Products { get; set; }
        public DbSet<PlanEntity> Plans { get; set; }
        public DbSet<SubscriptionEntity> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<UserAccountEntity>();
            modelBuilder.Ignore<AccessGroupEntity>();
            modelBuilder.Ignore<RoleEntity>();
            modelBuilder.Ignore<PermissionEntity>();

            modelBuilder.Entity<TenantEntity>(entity =>
            {
                entity.ToTable("tenants");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.Domain).IsUnique();
                entity.HasIndex(e => e.Document).IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Slug).HasColumnName("slug").IsRequired();
                entity.Property(e => e.Domain).HasColumnName("domain");
                entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasColumnType("varchar(20)");

                entity.Property(e => e.Settings)
                    .HasColumnName("settings")
                    .HasColumnType("jsonb")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, object>(),
                        new ValueComparer<Dictionary<string, object>>(
                            (c1, c2) => JsonSerializer.Serialize(c1, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c2, (JsonSerializerOptions?)null),
                            c => c == null ? 0 : JsonSerializer.Serialize(c, (JsonSerializerOptions?)null).GetHashCode(),
                            c => JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(c, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
                        ));

                entity.Property(e => e.ActiveSubscriptionId).HasColumnName("active_subscription_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

                // Campos corporativos
                entity.Property(e => e.Document).HasColumnName("document");
                entity.Property(e => e.RazaoSocial).HasColumnName("razao_social");
                entity.Property(e => e.InscricaoEstadual).HasColumnName("inscricao_estadual");
                entity.Property(e => e.InscricaoMunicipal).HasColumnName("inscricao_municipal");

                // Dados de contato
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Website).HasColumnName("website");

                entity.Property(e => e.AddressStreet).HasColumnName("address_street");
                entity.Property(e => e.AddressNumber).HasColumnName("address_number");
                entity.Property(e => e.AddressComplement).HasColumnName("address_complement");
                entity.Property(e => e.AddressNeighborhood).HasColumnName("address_neighborhood");
                entity.Property(e => e.AddressCity).HasColumnName("address_city");
                entity.Property(e => e.AddressState).HasColumnName("address_state");
                entity.Property(e => e.AddressZipcode).HasColumnName("address_zipcode");
                entity.Property(e => e.AddressCountry).HasColumnName("address_country");

                entity.Property(e => e.BillingStreet).HasColumnName("billing_street");
                entity.Property(e => e.BillingNumber).HasColumnName("billing_number");
                entity.Property(e => e.BillingComplement).HasColumnName("billing_complement");
                entity.Property(e => e.BillingNeighborhood).HasColumnName("billing_neighborhood");
                entity.Property(e => e.BillingCity).HasColumnName("billing_city");
                entity.Property(e => e.BillingState).HasColumnName("billing_state");
                entity.Property(e => e.BillingZipcode).HasColumnName("billing_zipcode");
                entity.Property(e => e.BillingCountry).HasColumnName("billing_country");

                entity.Property(e => e.LegalRepresentativeName).HasColumnName("legal_representative_name");
                entity.Property(e => e.LegalRepresentativeCpf).HasColumnName("legal_representative_cpf");
                entity.Property(e => e.LegalRepresentativeEmail).HasColumnName("legal_representative_email");
                entity.Property(e => e.LegalRepresentativePhone).HasColumnName("legal_representative_phone");

                entity.HasOne(d => d.ActiveSubscription)
                    .WithMany()
                    .HasForeignKey(d => d.ActiveSubscriptionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(t => t.Subscriptions)
                    .WithOne(s => s.Tenant)
                    .HasForeignKey(s => s.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TenantProductEntity>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Slug).HasColumnName("slug").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Category).HasColumnName("category").HasConversion<string>();
                entity.Property(e => e.Version).HasColumnName("version");
                entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasColumnType("varchar(20)");
                entity.Property(e => e.ConfigurationSchema).HasColumnName("configuration_schema").HasColumnType("jsonb");
                entity.Property(e => e.PricingModel).HasColumnName("pricing_model").HasConversion<string>();
            entity.Property(e => e.BasePrice).HasColumnName("base_price").HasPrecision(10, 2);
                entity.Property(e => e.SetupFee).HasColumnName("setup_fee").HasPrecision(10, 2);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

                entity.HasMany(p => p.Subscriptions)
                    .WithOne(s => s.Product)
                    .HasForeignKey(s => s.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PlanEntity>(entity =>
            {
                entity.ToTable("plans");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Slug).HasColumnName("slug").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Price).HasColumnName("price").HasPrecision(10, 2);
                entity.Property(e => e.BillingCycle).HasColumnName("billing_cycle").HasConversion<string>().HasColumnType("text");
                entity.Property(e => e.MaxUsers).HasColumnName("max_users");
                entity.Property(e => e.MaxStorageGb).HasColumnName("max_storage_gb");
                entity.Property(e => e.Features).HasColumnName("features").HasColumnType("jsonb");
                entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasColumnType("varchar(20)");
                entity.Property(e => e.IsTrial).HasColumnName("is_trial");
                entity.Property(e => e.TrialPeriodDays).HasColumnName("trial_period_days");
                entity.Property(e => e.SortOrder).HasColumnName("sort_order");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

                entity.HasMany(p => p.Subscriptions)
                    .WithOne(s => s.Plan)
                    .HasForeignKey(s => s.PlanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SubscriptionEntity>(entity =>
            {
                entity.ToTable("subscriptions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.TenantId, e.ProductId }).IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
                entity.Property(e => e.ProductId).HasColumnName("product_id").IsRequired();
                entity.Property(e => e.PlanId).HasColumnName("plan_id");
                entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasColumnType("varchar(20)");
                entity.Property(e => e.TrialEndsAt).HasColumnName("trial_ends_at").HasColumnType("timestamp with time zone");
                entity.Property(e => e.CurrentPeriodStart).HasColumnName("current_period_start").HasColumnType("timestamp with time zone");
                entity.Property(e => e.CurrentPeriodEnd).HasColumnName("current_period_end").HasColumnType("timestamp with time zone");
                entity.Property(e => e.CancelAtPeriodEnd).HasColumnName("cancel_at_period_end").HasDefaultValue(false);
                entity.Property(e => e.CancelledAt).HasColumnName("cancelled_at").HasColumnType("timestamp with time zone");
                entity.Property(e => e.CustomPricing).HasColumnName("custom_pricing").HasPrecision(10, 2);
                entity.Property(e => e.UsageLimits).HasColumnName("usage_limits").HasColumnType("jsonb");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

                // Relacionamentos
                entity.HasOne(d => d.Tenant)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TenantBusinessEntity>(entity =>
            {
                entity.ToTable("tenant_business_infos");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
                entity.Property(e => e.LogoUrl).HasColumnName("logo_url").HasMaxLength(500);
                entity.Property(e => e.BannerUrl).HasColumnName("banner_url").HasMaxLength(500);
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.InstagramUrl).HasColumnName("instagram_url").HasMaxLength(255);
                entity.Property(e => e.FacebookUrl).HasColumnName("facebook_url").HasMaxLength(255);
                entity.Property(e => e.WhatsappNumber).HasColumnName("whatsapp_number").HasMaxLength(20);
                
                entity.Property(e => e.OpeningHours).HasColumnName("opening_hours").HasColumnType("jsonb");
                entity.Property(e => e.PaymentMethods).HasColumnName("payment_methods").HasColumnType("jsonb");
                
                entity.Property(e => e.Latitude).HasColumnName("latitude");
                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.HasOne(e => e.Tenant)
                    .WithOne(t => t.BusinessInfo)
                    .HasForeignKey<TenantBusinessEntity>(e => e.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TenantModuleEntity>(entity =>
            {
                entity.ToTable("tenant_modules");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.TenantId, e.ModuleId }).IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
                entity.Property(e => e.ModuleId).HasColumnName("module_id").IsRequired();
                entity.Property(e => e.IsEnabled).HasColumnName("is_enabled").HasDefaultValue(true);
                entity.Property(e => e.Configuration).HasColumnName("configuration").HasColumnType("jsonb");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp with time zone");

                entity.Ignore(e => e.Module); // Ignorar navegação para outro contexto

                entity.HasOne(d => d.Tenant)
                    .WithMany()
                    .HasForeignKey(d => d.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PlanModuleEntity>(entity =>
            {
                entity.ToTable("plan_modules");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.PlanId, e.ModuleId }).IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.PlanId).HasColumnName("plan_id").IsRequired();
                entity.Property(e => e.ModuleId).HasColumnName("module_id").IsRequired();
                entity.Property(e => e.IsIncluded).HasColumnName("is_included").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp with time zone").HasDefaultValueSql("NOW()");

                entity.Ignore(e => e.Module); // Ignorar navegação para outro contexto

                entity.HasOne(d => d.Plan)
                    .WithMany()
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);

            modelBuilder.MultiTenantSeed();
        }
    }
}

