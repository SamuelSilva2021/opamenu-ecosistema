using Microsoft.EntityFrameworkCore;
using OpaMenu.Desktop.Models.Entities;
using System;
using System.IO;

namespace OpaMenu.Desktop.Models.Data;

/// <summary>
/// Contexto do Entity Framework Core que irá gerenciar o banco de dados local SQLite
/// </summary>
public class AppDbContext : DbContext
{
    // Tabelas Offline
    public DbSet<LocalOrderEntity> LocalOrders { get; set; }
    public DbSet<LocalOrderItemEntity> LocalOrderItems { get; set; }
    public DbSet<PrinterMappingEntity> PrinterMappings { get; set; }
    public DbSet<PrintJobEntity> PrintJobs { get; set; }
    // public DbSet<LocalProduct> LocalProducts { get; set; } // Adicionar dps
    // public DbSet<LocalCategory> LocalCategories { get; set; } // Adicionar dps

    public AppDbContext()
    {
        // Necessário caso seja instanciado sem injecao de dependencia via code
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Cria um diretório de dados locais do aplicativo (ex: AppData/Roaming/OpaMenuDesktop)
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var appDataPath = Path.Join(path, "OpaMenuDesktop");

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            var dbPath = Path.Join(appDataPath, "opamenu_gestor.db");
            
            // Configura o provedor do SQLite
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeamentos
        modelBuilder.Entity<LocalOrderEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.LocalId).IsUnique(); // LocalId gerado no WPF
            entity.Property(e => e.PayloadJson).IsRequired();
            
            // Índices para facilitar buscas de sincronização
            entity.HasIndex(e => e.SyncStatus);
        });

        modelBuilder.Entity<LocalOrderItemEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.LocalOrder)
                  .WithMany()
                  .HasForeignKey(e => e.LocalOrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PrinterMappingEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Destination).IsUnique();
        });

        modelBuilder.Entity<PrintJobEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.Status, e.CreatedAt });
        });
    }
}
