using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace OpaMenu.Infrastructure.Shared.Data.Context;

public class OpamenuDbContextFactory : IDesignTimeDbContextFactory<OpamenuDbContext>
{
    public OpamenuDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        
        // Se estiver rodando do diretório do projeto, use-o. 
        // Caso contrário, tente subir níveis até encontrar a raiz da solução ou projeto.
        // Neste caso específico, vamos assumir que as configurações podem vir de um appsettings.json local ou hardcoded para design time.
        
        var builder = new DbContextOptionsBuilder<OpamenuDbContext>();
        
        // Configuração para design-time. Ajuste a string de conexão conforme seu ambiente de desenvolvimento.
        // NOTA: Em produção, isso viria da injeção de dependência.
        var connectionString = "Host=localhost;Database=opamenu_business;Username=postgres;Password=postgres";
        
        builder.UseNpgsql(connectionString);

        return new OpamenuDbContext(builder.Options);
    }
}