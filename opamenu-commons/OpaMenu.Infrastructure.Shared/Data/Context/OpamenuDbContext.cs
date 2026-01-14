using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Infrastructure.Shared.Data.Context;

public class OpamenuDbContext(DbContextOptions<OpamenuDbContext> options) : DbContext(options)
{
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<ProductImageEntity> ProductImages { get; set; }
    // TODO: Add ProductPriceHistory and ProductActivityLog after migration
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemEntity> OrderItems { get; set; }
    public DbSet<OrderStatusHistoryEntity> OrderStatusHistory { get; set; }
    public DbSet<OrderRejectionEntity> OrderRejections { get; set; }
    public DbSet<PaymentEntity> Payments { get; set; }
    public DbSet<PaymentRefundEntity> PaymentRefunds { get; set; }
    public DbSet<AddonGroupEntity> AddonGroups { get; set; }
    public DbSet<AddonEntity> Addons { get; set; }
    public DbSet<ProductAddonGroupEntity> ProductAddonGroups { get; set; }
    public DbSet<OrderItemAddon> OrderItemAddons { get; set; }
    public DbSet<PaymentMethodEntity> PaymentMethods { get; set; }
    public DbSet<TenantPaymentMethodEntity> TenantPaymentMethods { get; set; }
    public DbSet<CouponEntity> Coupons { get; set; }
    public DbSet<TenantCustomerEntity> TenantCustomers { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<TableEntity> Tables { get; set; }

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
            
            entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
        });

        // Order configuration
        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.CustomerPhone)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.CustomerEmail)
                .HasMaxLength(100);
            entity.Property(e => e.DeliveryAddress)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .IsRequired();
            entity.Property(e => e.DeliveryFee)
                .HasPrecision(10, 2)
                .IsRequired();
            entity.Property(e => e.Total)
                .HasPrecision(10, 2)
                .IsRequired();
            entity.Property(e => e.Status)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();
            entity.Property(e => e.Notes)
                .HasMaxLength(1000);
            entity.Property(e => e.IsDelivery)
                .IsRequired();
            entity.Property(e => e.OrderType)
                .IsRequired();
            entity.Property(e => e.TableId)
                .IsRequired(false);

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.CustomerPhone);
            entity.HasIndex(e => e.OrderType);
            entity.HasIndex(e => e.TableId);

            entity.HasOne(e => e.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(e => e.TableId)
                .OnDelete(DeleteBehavior.SetNull);
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
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .IsRequired();
            entity.Property(e => e.Quantity)
                .IsRequired();
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .IsRequired();
            entity.Property(e => e.Notes)
                .HasMaxLength(500);

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

        // OrderStatusHistory configuration
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
            entity.Property(e => e.OrderId)
                .IsRequired();
            entity.Property(e => e.Amount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.Method)
                .IsRequired();
            entity.Property(e => e.Status)
                .IsRequired();
            entity.Property(e => e.GatewayTransactionId)
                .HasMaxLength(200);
            entity.Property(e => e.GatewayResponse)
                .HasMaxLength(1000);
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.Notes)
                .HasMaxLength(500);

            entity.HasOne(e => e.Order)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Refunds)
                .WithOne(e => e.Payment)
                .HasForeignKey(e => e.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.GatewayTransactionId);
        });

        // PaymentRefund configuration
        modelBuilder.Entity<PaymentRefundEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PaymentId)
                .IsRequired();
            entity.Property(e => e.Amount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            entity.Property(e => e.Reason)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.RefundedAt)
                .IsRequired();
            entity.Property(e => e.RefundedBy)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.GatewayRefundId)
                .HasMaxLength(200);
            entity.Property(e => e.GatewayResponse)
                .HasMaxLength(1000);

            entity.HasOne(e => e.Payment)
                .WithMany(e => e.Refunds)
                .HasForeignKey(e => e.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PaymentId);
            entity.HasIndex(e => e.RefundedAt);
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
        modelBuilder.Entity<OrderItemAddon>(entity =>
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
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            entity.Property(e => e.IconUrl)
                .HasMaxLength(500);
            entity.Property(e => e.IsActive)
                .IsRequired();
            entity.Property(e => e.IsOnline)
                .IsRequired();
            entity.Property(e => e.DisplayOrder)
                .IsRequired();

            entity.HasIndex(e => e.Slug)
                .IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsOnline);
            entity.HasIndex(e => e.DisplayOrder);
        });

        // TenantPaymentMethod configuration
        modelBuilder.Entity<TenantPaymentMethodEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // TenantId from BaseEntity
            entity.Property(e => e.TenantId)
                .IsRequired();

            entity.Property(e => e.PaymentMethodId)
                .IsRequired();

            entity.Property(e => e.Alias)
                .HasMaxLength(100);

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Property(e => e.Configuration)
                .HasMaxLength(500);

            entity.Property(e => e.DisplayOrder)
                .IsRequired();

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

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed categories
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<CategoryEntity>().HasData(
            new CategoryEntity
            {
                Id = 1,
                Name = "Pratos Principais",
                Description = "Pratos principais do cardÃ¡pio",
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new CategoryEntity
            {
                Id = 2,
                Name = "Bebidas",
                Description = "Bebidas e refrescos",
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new CategoryEntity
            {
                Id = 3,
                Name = "Sobremesas",
                Description = "Doces e sobremesas",
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new CategoryEntity
            {
                Id = 4,
                Name = "Entradas",
                Description = "Aperitivos e entradas",
                DisplayOrder = 4,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed products
        modelBuilder.Entity<ProductEntity>().HasData(
            new ProductEntity
            {
                Id = 1,
                Name = "HambÃºrguer ClÃ¡ssico",
                Description = "HambÃºrguer artesanal com carne bovina, queijo, alface, tomate e molho especial",
                Price = 25.90m,
                CategoryId = 1,
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new ProductEntity
            {
                Id = 2,
                Name = "Pizza Margherita",
                Description = "Pizza tradicional com molho de tomate, mussarela e manjericÃ£o fresco",
                Price = 32.50m,
                CategoryId = 1,
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new ProductEntity
            {
                Id = 3,
                Name = "Refrigerante Lata",
                Description = "Refrigerante gelado em lata de 350ml - diversos sabores",
                Price = 5.50m,
                CategoryId = 2,
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new ProductEntity
            {
                Id = 4,
                Name = "Suco Natural",
                Description = "Suco natural de frutas frescas - 400ml",
                Price = 8.90m,
                CategoryId = 2,
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new ProductEntity
            {
                Id = 5,
                Name = "Pudim de Leite",
                Description = "Pudim caseiro de leite condensado com calda de aÃ§Ãºcar",
                Price = 12.90m,
                CategoryId = 3,
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new ProductEntity
            {
                Id = 6,
                Name = "PorÃ§Ã£o de Batata Frita",
                Description = "Batatas crocantes cortadas na casa, tempero especial - serve atÃ© 3 pessoas",
                Price = 18.50m,
                CategoryId = 4,
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

        // Seed payment methods
        modelBuilder.Entity<PaymentMethodEntity>().HasData(
            new PaymentMethodEntity
            {
                Id = 1,
                Name = "CrÃ©dito",
                Slug = "credito",
                Description = "Pagamento via cartÃ£o de crÃ©dito",
                IsActive = true,
                IsOnline = true,
                DisplayOrder = 1,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new PaymentMethodEntity
            {
                Id = 2,
                Name = "DÃ©bito",
                Slug = "debito",
                Description = "Pagamento via cartÃ£o de dÃ©bito",
                IsActive = true,
                IsOnline = true,
                DisplayOrder = 2,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new PaymentMethodEntity
            {
                Id = 3,
                Name = "PIX",
                Slug = "pix",
                Description = "Pagamento instantÃ¢neo via PIX",
                IsActive = true,
                IsOnline = true,
                DisplayOrder = 3,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new PaymentMethodEntity
            {
                Id = 4,
                Name = "Dinheiro",
                Slug = "dinheiro",
                Description = "Pagamento em espÃ©cie",
                IsActive = true,
                IsOnline = false,
                DisplayOrder = 4,
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        );

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

        // TODO: Add ProductPriceHistory and ProductActivityLog configuration after migration
        /*
        // ProductPriceHistory configuration
        modelBuilder.Entity<ProductPriceHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductId)
                .IsRequired();
            entity.Property(e => e.PreviousPrice)
                .HasPrecision(10, 2)
                .IsRequired();
            entity.Property(e => e.NewPrice)
                .HasPrecision(10, 2)
                .IsRequired();
            entity.Property(e => e.ChangedAt)
                .IsRequired();
            entity.Property(e => e.ChangedBy)
                .HasMaxLength(100);
            entity.Property(e => e.Reason)
                .HasMaxLength(200);

            // Relationships
            entity.HasOne(e => e.Product)
                .WithMany(p => p.PriceHistory)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.ChangedAt);
        });

        // ProductActivityLog configuration
        modelBuilder.Entity<ProductActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductId)
                .IsRequired();
            entity.Property(e => e.Action)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.PreviousValue)
                .HasMaxLength(200);
            entity.Property(e => e.NewValue)
                .HasMaxLength(200);
            entity.Property(e => e.Timestamp)
                .IsRequired();
            entity.Property(e => e.UserId)
                .HasMaxLength(100);

            // Relationships
            entity.HasOne(e => e.Product)
                .WithMany(p => p.ActivityLogs)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.Timestamp);
        });
        */
    }
}

