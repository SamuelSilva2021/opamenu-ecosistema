using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace OpaMenu.Desktop.Models;

/// <summary>
/// Contexto do Entity Framework Core que irá gerenciar o banco de dados local SQLite
/// </summary>
public class AppDbContext : DbContext
{
    // Tabelas Offline
    public DbSet<LocalOrder> LocalOrders { get; set; }
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
        modelBuilder.Entity<LocalOrder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.LocalId).IsUnique(); // LocalId gerado no WPF
            entity.Property(e => e.PayloadJson).IsRequired();
            
            // Índices para facilitar buscas de sincronização
            entity.HasIndex(e => e.SyncStatus);
        });
    }
}