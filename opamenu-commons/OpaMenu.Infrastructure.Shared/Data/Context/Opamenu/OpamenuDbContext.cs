using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;

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
    public DbSet<AditionalGroupEntity> AditionalGroups { get; set; }
    public DbSet<AditionalEntity> Aditionals { get; set; }
    public DbSet<ProductAditionalGroupEntity> ProductAditionalGroups { get; set; }
    public DbSet<OrderItemAditionalEntity> OrderItemAditionals { get; set; }
    public DbSet<PaymentMethodEntity> PaymentMethods { get; set; }
    public DbSet<TenantPaymentConfigEntity> TenantPaymentConfigs { get; set; }
    public DbSet<TenantPaymentMethodEntity> TenantPaymentMethods { get; set; }
    public DbSet<CouponEntity> Coupons { get; set; }
    public DbSet<TenantCustomerEntity> TenantCustomers { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<TableEntity> Tables { get; set; }
    public DbSet<LoyaltyProgramEntity> LoyaltyPrograms { get; set; }
    public DbSet<LoyaltyTransactionEntity> LoyaltyTransactions { get; set; }
    public DbSet<CustomerLoyaltyBalanceEntity> CustomerLoyaltyBalances { get; set; }
    public DbSet<CollaboratorEntity> Collaborators { get; set; }
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

            entity.HasOne(e => e.Driver)
                .WithMany()
                .HasForeignKey(e => e.DriverId)
                .OnDelete(DeleteBehavior.SetNull);

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
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Method).HasConversion<string>().IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().IsRequired();
            entity.Property(e => e.Provider).HasConversion<string>().IsRequired();
            entity.Property(e => e.QrCode).HasColumnType("text");
            entity.Property(e => e.GatewayTransactionId).HasMaxLength(200);
            entity.Property(e => e.ProviderPaymentId).HasMaxLength(200);
            entity.Property(e => e.GatewayResponse).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Order).WithMany(e => e.Payments).HasForeignKey(e => e.OrderId);

            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.GatewayTransactionId);
            entity.HasIndex(e => e.ProviderPaymentId);
            entity.HasIndex(e => new { e.TenantId, e.OrderId });
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

        // Collaborator configuration
        modelBuilder.Entity<CollaboratorEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).HasConversion<string>().IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Active).IsRequired();
            entity.Property(e => e.UserAccountId).IsRequired(false);

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.UserAccountId);
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

        // Aditional configuration
        modelBuilder.Entity<AditionalEntity>(entity =>
        {
            entity.HasOne(d => d.AditionalGroup)
                  .WithMany(p => p.Aditionals)
                  .HasForeignKey(d => d.AditionalGroupId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ProductAditionalGroup configuration
        modelBuilder.Entity<ProductAditionalGroupEntity>(entity =>
        {
            entity.HasOne(d => d.Product)
                  .WithMany(p => p.AditionalGroups)
                  .HasForeignKey(d => d.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.AditionalGroup)
                  .WithMany(p => p.ProductAditionalGroups)
                  .HasForeignKey(d => d.AditionalGroupId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ProductId, e.AditionalGroupId }).IsUnique();
        });

        // OrderItemAditional configuration
        modelBuilder.Entity<OrderItemAditionalEntity>(entity =>
        {
            entity.HasOne(d => d.OrderItem)
                  .WithMany(p => p.Aditionals)
                  .HasForeignKey(d => d.OrderItemId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Aditional)
                  .WithMany(p => p.OrderItemAditionals)
                  .HasForeignKey(d => d.AditionalId)
                  .OnDelete(DeleteBehavior.Restrict); // Não deletar adicional se usado em pedido
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

            entity.Property(e => e.Configuration).HasColumnType("jsonb");

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

        modelBuilder.Entity<PaymentTransactionEntity>(e =>
        {
            e.HasIndex(t => t.ProviderEventId);
            e.Property(p => p.RawPayload).HasColumnType("jsonb");
        });
        
        modelBuilder.Entity<TenantPaymentConfigEntity>(entity =>
        {
            entity.Property(e => e.Provider).HasConversion<string>().IsRequired();
            entity.Property(e => e.PaymentMethod).HasConversion<string>().IsRequired();
        });

        modelBuilder.OpamenuSeed();
    }
}

