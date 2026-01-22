using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;

namespace OpaMenu.Web.UserEntry;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")] // Apenas administradores podem acessar migraÃ§Ãµes
public class MigrationController : ControllerBase
{
    private readonly OpamenuDbContext _context;
    private readonly ILogger<MigrationController> _logger;

    public MigrationController(
        OpamenuDbContext context,
        ILogger<MigrationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Migra os caminhos das imagens dos produtos existentes
    /// </summary>
    [HttpPost("fix-image-paths")]
    public async Task<ActionResult<object>> FixImagePaths()
    {
        try
        {
            var products = await _context.Products
                .Where(p => !string.IsNullOrEmpty(p.ImageUrl) && 
                           !p.ImageUrl.Contains('/') && 
                           p.CreatedAt >= new DateTime(2025, 8, 1))
                .ToListAsync();

            var updatedCount = 0;

            foreach (var product in products)
            {
                // Assumir que as imagens estÃ£o em products/2025/08/ baseado na data de criaÃ§Ã£o
                var year = product.CreatedAt.Year;
                var month = product.CreatedAt.Month.ToString("00");
                
                var newImageUrl = $"products/{year}/{month}/{product.ImageUrl}";
                
                _logger.LogInformation("Updating product {ProductId}: {OldPath} -> {NewPath}", 
                    product.Id, product.ImageUrl, newImageUrl);
                
                product.ImageUrl = newImageUrl;
                updatedCount++;
            }

            await _context.SaveChangesAsync();

            return Ok(new 
            {
                Success = true,
                Message = $"Updated {updatedCount} products",
                UpdatedProducts = products.Select(p => new 
                {
                    p.Id,
                    p.Name,
                    p.ImageUrl
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fixing image paths");
            return StatusCode(500, new { Success = false, Message = "Error fixing image paths" });
        }
    }
}

