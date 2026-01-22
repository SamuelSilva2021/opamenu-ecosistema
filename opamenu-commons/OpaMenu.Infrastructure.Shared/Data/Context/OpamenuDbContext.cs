using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Infrastructure.Shared.Data.Context;

public class OpamenuDbContext(DbContextOptions<OpamenuDbContext> options) : DbContext(options)
{
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ProductImageEntity> ProductImages { get; set; }
    // TODO: Add ProductPriceHistory and ProductActivityLog after migration
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemEntity> OrderItems { get; set; }
    public DbSet<OrderStatusHistoryEntity> EOrderStatusHistory { get; set; }
    public DbSet<OrderRejectionEntity> OrderRejections { get; set; }
    public DbSet<PaymentEntity> Payments { get; set; }
    public DbSet<PaymentRefundEntity> PaymentRefunds { get; set; }
    public DbSet<AddonGroupEntity> AddonGroups { get; set; }
    public DbSet<AddonEntity> Addons { get; set; }
    public DbSet<ProductAddonGroupEntity> ProductAddonGroups { get; set; }
    public DbSet<OrderItemAddonEntity> OrderItemAddons { get; set; }
    public DbSet<PaymentMethodEntity> PaymentMethods { get; set; }
    public DbSet<TenantPaymentMethodEntity> TenantPaymentMethods { get; set; }
    public DbSet<CouponEntity> Coupons { get; set; }
    public DbSet<TenantCustomerEntity> TenantCustomers { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<TableEntity> Tables { get; set; }
    public DbSet<LoyaltyProgramEntity> LoyaltyPrograms { get; set; }
    public DbSet<LoyaltyTransactionEntity> LoyaltyTransactions { get; set; }
    public DbSet<CustomerLoyaltyBalanceEntity> CustomerLoyaltyBalances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category configuration
        modelBuilder.Entity<CategoryEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            entity.Property(e => e.DisplayOrder)
                .IsRequired();
            entity.Property(e => e.IsActive)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            entity.HasIndex(e => e.DisplayOrder);
            entity.HasIndex(e => e.IsActive);

            entity.HasMany(e => e.Products)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Product configuration
        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
            entity.Property(e => e.Price)
                .IsRequired()
                .HasPrecision(10, 2);
            entity.Property(e => e.DisplayOrder)
                .IsRequired();
            entity.Property(e => e.IsActive)
                .IsRequired();
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.DisplayOrder);
        });

        // Coupon configuration
        modelBuilder.Entity<CouponEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.DiscountValue).HasPrecision(10, 2);
            entity.Property(e => e.MinOrderValue).HasPrecision(10, 2);
            entity.Property(e => e.MaxDiscountValue).HasPrecision(10, 2);
            entity.Property(e => e.DiscountType).HasConversion<string>().IsRequired();

            entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
        });

        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.ToTable("orders");

            entity.HasKey(e => e.Id);

            // =========================
            // Customer
            // =========================
            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CustomerPhone)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(100);

            entity.Property(e => e.CustomerId)
                .IsRequired();

            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Address & Notes
            // =========================
            entity.Property(e => e.DeliveryAddress)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Notes)
                .HasMaxLength(1000);

            // =========================
            // Financial
            // =========================
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .IsRequired();

            entity.Property(e => e.DeliveryFee)
                .HasPrecision(10, 2)
                .IsRequired();

            entity.Property(e => e.DiscountAmount)
                .HasPrecision(10, 2)
                .HasDefaultValue(0);

            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .IsRequired();

            entity.Property(e => e.CouponCode)
                .HasMaxLength(50);

            // =========================
            // Status & Type (ENUMS)
            // =========================
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(e => e.OrderType)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(e => e.IsDelivery)
                .IsRequired();

            // =========================
            // Table / Queue
            // =========================
            entity.Property(e => e.TableId)
                .IsRequired(false);

            entity.HasOne(e => e.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(e => e.TableId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Property(e => e.QueuePosition)
                .HasDefaultValue(0);

            // =========================
            // Estimates
            // =========================
            entity.Property(e => e.EstimatedPreparationMinutes)
                .IsRequired(false);

            entity.Property(e => e.EstimatedDeliveryTime)
                .IsRequired(false);

            // =========================
            // Dates
            // =========================
            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // =========================
            // Relationships
            // =========================
            entity.HasMany(e => e.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.StatusHistory)
                .WithOne(h => h.Order)
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Rejection)
                .WithOne(r => r.Order)
                .HasForeignKey<OrderRejectionEntity>(r => r.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Indexes
            // =========================
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.CustomerPhone);
            entity.HasIndex(e => e.OrderType);
            entity.HasIndex(e => e.TableId);
        });

        // Table configuration
        modelBuilder.Entity<TableEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Capacity)
                .IsRequired();
            entity.Property(e => e.IsActive)
                .IsRequired();
            entity.Property(e => e.QrCodeUrl)
                .HasMaxLength(500);
            
            entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItemEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Subtotal).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);

            // Relationships
            entity.HasOne(e => e.Order)
                .WithMany(e => e.Items)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProductId);
        });

        // EOrderStatusHistory configuration
        modelBuilder.Entity<OrderStatusHistoryEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderId)
                .IsRequired();
            entity.Property(e => e.Status)
                .IsRequired();
            entity.Property(e => e.Timestamp)
                .IsRequired();
            entity.Property(e => e.Notes)
                .HasMaxLength(500);
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(e => e.Order)
                .WithMany(e => e.StatusHistory)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Timestamp);
        });

        // OrderRejection configuration
        modelBuilder.Entity<OrderRejectionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderId)
                .IsRequired();
            entity.Property(e => e.Reason)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Notes)
                .HasMaxLength(500);
            entity.Property(e => e.RejectedAt)
                .IsRequired();
            entity.Property(e => e.RejectedBy)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(e => e.Order)
                .WithOne(e => e.Rejection)
                .HasForeignKey<OrderRejectionEntity>(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.RejectedAt);
        });

        // Payment configuration
        modelBuilder.Entity<PaymentEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Method).HasConversion<string>().IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().IsRequired();
            entity.Property(e => e.GatewayTransactionId).HasMaxLength(200);
            entity.Property(e => e.GatewayResponse).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Order).WithMany(e => e.Payments).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Refunds).WithOne(e => e.Payment).HasForeignKey(e => e.PaymentId).OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.GatewayTransactionId);
        });

        // PaymentRefund configuration
        modelBuilder.Entity<PaymentRefundEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PaymentId).IsRequired();
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(500);
            entity.Property(e => e.RefundedAt).IsRequired();
            entity.Property(e => e.RefundedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.GatewayRefundId).HasMaxLength(200);
            entity.Property(e => e.GatewayResponse).HasMaxLength(1000);

            entity.HasOne(e => e.Payment).WithMany(e => e.Refunds).HasForeignKey(e => e.PaymentId).OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PaymentId);
            entity.HasIndex(e => e.RefundedAt);
        });

        // Loyalty Program configuration
        modelBuilder.Entity<LoyaltyProgramEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.PointsPerCurrency).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.CurrencyValue).HasPrecision(10, 2).IsRequired();
            entity.Property(e => e.MinOrderValue).HasPrecision(10, 2).IsRequired();
            
            // Um programa por Tenant
            entity.HasIndex(e => e.TenantId).IsUnique();
        });

        // Customer Loyalty Balance configuration
        modelBuilder.Entity<CustomerLoyaltyBalanceEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.Balance).IsRequired();
            entity.Property(e => e.TotalEarned).IsRequired();
            
            // Saldo único por Cliente e Tenant
            entity.HasIndex(e => new { e.TenantId, e.CustomerId }).IsUnique();

            entity.HasOne(e => e.Customer)
                .WithMany() // Assumindo que Customer não precisa navegar para Balances por enquanto
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Loyalty Transaction configuration
        modelBuilder.Entity<LoyaltyTransactionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerLoyaltyBalanceId).IsRequired();
            entity.Property(e => e.Points).IsRequired();
            entity.Property(e => e.Type).HasConversion<string>().IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);
            
            entity.HasOne(e => e.CustomerLoyaltyBalance)
                .WithMany(b => b.Transactions)
                .HasForeignKey(e => e.CustomerLoyaltyBalanceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Order)
                .WithMany()
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.CustomerLoyaltyBalanceId);
        });

        // Addon configuration
        modelBuilder.Entity<AddonEntity>(entity =>
        {
            entity.HasOne(d => d.AddonGroup)
                  .WithMany(p => p.Addons)
                  .HasForeignKey(d => d.AddonGroupId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ProductAddonGroup configuration
        modelBuilder.Entity<ProductAddonGroupEntity>(entity =>
        {
            entity.HasOne(d => d.Product)
                  .WithMany(p => p.AddonGroups)
                  .HasForeignKey(d => d.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.AddonGroup)
                  .WithMany(p => p.ProductAddonGroups)
                  .HasForeignKey(d => d.AddonGroupId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ProductId, e.AddonGroupId }).IsUnique();
        });

        // OrderItemAddon configuration
        modelBuilder.Entity<OrderItemAddonEntity>(entity =>
        {
            entity.HasOne(d => d.OrderItem)
                  .WithMany(p => p.Addons)
                  .HasForeignKey(d => d.OrderItemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Addon)
                  .WithMany(p => p.OrderItemAddons)
                  .HasForeignKey(d => d.AddonId)
                  .OnDelete(DeleteBehavior.Restrict); // NÃ£o deletar addon se usado em pedido
        });

        // PaymentMethod configuration
        modelBuilder.Entity<PaymentMethodEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IconUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.IsOnline).IsRequired();
            entity.Property(e => e.DisplayOrder).IsRequired();

            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsOnline);
            entity.HasIndex(e => e.DisplayOrder);
        });

        // TenantPaymentMethod configuration
        modelBuilder.Entity<TenantPaymentMethodEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // TenantId from BaseEntity
            entity.Property(e => e.TenantId).IsRequired();

            entity.Property(e => e.PaymentMethodId).IsRequired();

            entity.Property(e => e.Alias).HasMaxLength(100);

            entity.Property(e => e.IsActive).IsRequired();

            entity.Property(e => e.Configuration).HasMaxLength(500);

            entity.Property(e => e.DisplayOrder).IsRequired();

            // Relationship with PaymentMethod
            entity.HasOne(e => e.PaymentMethod)
                .WithMany(p => p.TenantConfiguredMethods)
                .HasForeignKey(e => e.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index to ensure unique payment method per tenant
            entity.HasIndex(e => new { e.TenantId, e.PaymentMethodId })
                .IsUnique();
        });

        // TenantCustomer configuration
        modelBuilder.Entity<TenantCustomerEntity>(entity =>
        {
            entity.ToTable("tenant_customers");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.TenantId, x.CustomerId })
                   .IsUnique();

            entity.HasOne(x => x.Customer)
                   .WithMany(c => c.TenantCustomers)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        // Customer configuration
        modelBuilder.Entity<CustomerEntity>(entity =>
        {
            entity.ToTable("customers");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Email);
            entity.HasIndex(x => x.Phone);
        });

        // ProductImage configuration
        modelBuilder.Entity<ProductImageEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.OriginalName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.FilePath)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.MimeType)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.FileSize)
                .IsRequired();
            entity.Property(e => e.AspectRatio)
                .HasMaxLength(50);
            entity.Property(e => e.UploadDate)
                .IsRequired();
            entity.Property(e => e.ThumbnailPath)
                .HasMaxLength(500);
            entity.Property(e => e.MediumPath)
                .HasMaxLength(500);
            entity.Property(e => e.LargePath)
                .HasMaxLength(500);

            // Foreign key relationship
            entity.HasOne(e => e.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.IsPrimary);
            entity.HasIndex(e => e.UploadDate);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var categoryId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        var productId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        var addonGroupId = Guid.Parse("00000000-0000-0000-0000-000000000004");
        var customerId = Guid.Parse("00000000-0000-0000-0000-000000000005");
        var orderId = Guid.Parse("00000000-0000-0000-0000-000000000006");

        // Seed payment methods
        modelBuilder.Entity<PaymentMethodEntity>().HasData(
            new PaymentMethodEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000010"),
                Name = "Crédito",
                Slug = "credito",
                Description = "Pagamento via cartão de crédito",
                IsActive = true,
                IsOnline = true,
                DisplayOrder = 1,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new PaymentMethodEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
                Name = "Débito",
                Slug = "debito",
                Description = "Pagamento via cartão de débito",
                IsActive = true,
                IsOnline = true,
                DisplayOrder = 2,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new PaymentMethodEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
                Name = "PIX",
                Slug = "pix",
                Description = "Pagamento via PIX",
                IsActive = true,
                IsOnline = true,
                DisplayOrder = 3,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new PaymentMethodEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000013"),
                Name = "Dinheiro",
                Slug = "dinheiro",
                Description = "Pagamento em dinheiro",
                IsActive = true,
                IsOnline = false,
                DisplayOrder = 4,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed categories
        modelBuilder.Entity<CategoryEntity>().HasData(
            new CategoryEntity
            {
                Id = categoryId,
                Name = "Lanches",
                Description = "Lanches diversos",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed products
        modelBuilder.Entity<ProductEntity>().HasData(
            new ProductEntity
            {
                Id = productId,
                Name = "Hamburguer Clássico",
                Description = "Hamburguer artesanal com carne bovina, queijo, alface, tomate e molho especial",
                Price = 25.90m,
                CategoryId = categoryId,
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed addon groups
        modelBuilder.Entity<AddonGroupEntity>().HasData(
            new AddonGroupEntity
            {
                Id = addonGroupId,
                Name = "Complementos do Hamburguer",
                Description = "Escolha seus complementos favoritos",
                IsRequired = false,
                MinSelections = 0,
                MaxSelections = 3,
                DisplayOrder = 1,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed addons
        modelBuilder.Entity<AddonEntity>().HasData(
            new AddonEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000020"),
                AddonGroupId = addonGroupId,
                Name = "Bacon",
                Description = "Tiras crocantes de bacon",
                Price = 4.00m,
                DisplayOrder = 1,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new AddonEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000021"),
                AddonGroupId = addonGroupId,
                Name = "Cebola Caramelizada",
                Description = "Cebolas caramelizadas na manteiga",
                Price = 3.00m,
                DisplayOrder = 2,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new AddonEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000022"),
                AddonGroupId = addonGroupId,
                Name = "Queijo Extra",
                Description = "Adicione uma fatia extra de queijo",
                Price = 2.50m,
                DisplayOrder = 3,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed ProductAddonGroup relationship
        modelBuilder.Entity<ProductAddonGroupEntity>().HasData(
            new ProductAddonGroupEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000030"),
                ProductId = productId,
                AddonGroupId = addonGroupId,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed a table
        modelBuilder.Entity<TableEntity>().HasData(
            new TableEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000040"),
                Name = "Mesa 1",
                Capacity = 4,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed a customer
        modelBuilder.Entity<CustomerEntity>().HasData(
            new CustomerEntity
            {
                Id = customerId,
                Name = "Cliente Exemplo",
                Email = "exemplo@exemplo.com",
                Phone = "11999999999",
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            });

        // Seed a tenant-customer relationship
        modelBuilder.Entity<TenantCustomerEntity>().HasData(
            new TenantCustomerEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000050"),
                TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                CustomerId = customerId,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            });

        // Seed a loyalty program
        modelBuilder.Entity<LoyaltyProgramEntity>().HasData(
            new LoyaltyProgramEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000060"),
                TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Programa de Fidelidade Padrão",
                Description = "Ganhe pontos a cada compra e troque por descontos!",
                PointsPerCurrency = 1.0m,
                CurrencyValue = 0.10m,
                MinOrderValue = 20.00m,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            });

        // Seed a customer loyalty balance
        modelBuilder.Entity<CustomerLoyaltyBalanceEntity>().HasData(
            new CustomerLoyaltyBalanceEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000070"),
                TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                CustomerId = customerId,
                Balance = 0,
                TotalEarned = 0,
                LastActivityAt = seedDate,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            });

        // Seed a Order
        modelBuilder.Entity<OrderEntity>().HasData(
            new OrderEntity
            {
                Id = orderId,
                TenantId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                CustomerId = customerId,
                CustomerName = "Cliente Exemplo",
                CustomerPhone = "11999999999",
                DeliveryAddress = "Rua Exemplo, 123, Bairro, Cidade, Estado",
                Subtotal = 25.90m,
                DeliveryFee = 5.00m,
                Total = 30.90m,
                Status = EOrderStatus.Pending,
                OrderType = EOrderType.Delivery,
                IsDelivery = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            });

        // Seed an OrderItem
        modelBuilder.Entity<OrderItemEntity>().HasData(
            new OrderItemEntity
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000080"),
                OrderId = orderId,
                ProductId = productId,
                ProductName = "Hamburguer Clássico",
                UnitPrice = 25.90m,
                Quantity = 1,
                Subtotal = 25.90m,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            });
    }
}

