using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.MultiTenant;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using OpaMenu.Infrastructure.Shared.Enums.MultiTenant;

namespace OpaMenu.Web.UserEntry.MultiTenant;

[ApiController]
[Route("api/tenant-products")]
[Authorize(Roles = "SUPER_ADMIN")]
public sealed class TenantProductsController(MultiTenantDbContext dbContext) : ControllerBase
{
    private readonly MultiTenantDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<List<TenantProductDto>>> GetAll()
    {
        var products = await _dbContext.Products.AsNoTracking().ToListAsync();
        var ids = products.Select(p => p.Id).ToList();

        var agg = await _dbContext.Subscriptions.AsNoTracking()
            .Where(s => ids.Contains(s.ProductId))
            .GroupBy(s => s.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Total = g.Count(),
                Active = g.Count(x => x.Status == ESubscriptionStatus.Ativo || x.Status == ESubscriptionStatus.Trial)
            })
            .ToListAsync();

        var aggByProduct = agg.ToDictionary(x => x.ProductId, x => x);

        var result = products
            .OrderBy(p => p.Name)
            .Select(p =>
            {
                aggByProduct.TryGetValue(p.Id, out var a);
                return new TenantProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Description = p.Description,
                    Category = p.Category.ToString(),
                    Version = p.Version,
                    Status = p.Status.ToString(),
                    ConfigurationSchema = p.ConfigurationSchema,
                    PricingModel = p.PricingModel.ToString(),
                    BasePrice = p.BasePrice,
                    SetupFee = p.SetupFee,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    TotalSubscriptions = a?.Total ?? 0,
                    ActiveSubscriptions = a?.Active ?? 0
                };
            })
            .ToList();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenantProductDto>> GetById([FromRoute] Guid id)
    {
        var product = await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        var total = await _dbContext.Subscriptions.AsNoTracking().CountAsync(s => s.ProductId == id);
        var active = await _dbContext.Subscriptions.AsNoTracking().CountAsync(s => s.ProductId == id && (s.Status == ESubscriptionStatus.Ativo || s.Status == ESubscriptionStatus.Trial));

        return Ok(new TenantProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            Category = product.Category.ToString(),
            Version = product.Version,
            Status = product.Status.ToString(),
            ConfigurationSchema = product.ConfigurationSchema,
            PricingModel = product.PricingModel.ToString(),
            BasePrice = product.BasePrice,
            SetupFee = product.SetupFee,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            TotalSubscriptions = total,
            ActiveSubscriptions = active
        });
    }

    [HttpPost]
    public async Task<ActionResult<TenantProductDto>> Create([FromBody] CreateTenantProductRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Slug) || string.IsNullOrWhiteSpace(request.Category))
            return BadRequest();
        

        if (!Enum.TryParse<ETenantProductCategory>(request.Category, ignoreCase: true, out var category))
            return BadRequest();

        var status = EProductStatus.Ativo;
        if (request.Status != null && !Enum.TryParse<EProductStatus>(request.Status, ignoreCase: true, out status))
            return BadRequest();

        var pricing = ETenantProductPricingModel.Assinatura;
        if (request.PricingModel != null && !Enum.TryParse<ETenantProductPricingModel>(request.PricingModel, ignoreCase: true, out pricing))
            return BadRequest();

        var slug = request.Slug.Trim();
        var exists = await _dbContext.Products.AsNoTracking().AnyAsync(p => p.Slug == slug);
        if (exists)
            return BadRequest();

        var entity = new TenantProductEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = slug,
            Description = request.Description,
            Category = category,
            Version = string.IsNullOrWhiteSpace(request.Version) ? "1.0.0" : request.Version.Trim(),
            Status = status,
            ConfigurationSchema = request.ConfigurationSchema,
            PricingModel = pricing,
            BasePrice = request.BasePrice,
            SetupFee = request.SetupFee ?? 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _dbContext.Products.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(new TenantProductDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Slug = entity.Slug,
            Description = entity.Description,
            Category = entity.Category.ToString(),
            Version = entity.Version,
            Status = entity.Status.ToString(),
            ConfigurationSchema = entity.ConfigurationSchema,
            PricingModel = entity.PricingModel.ToString(),
            BasePrice = entity.BasePrice,
            SetupFee = entity.SetupFee,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            TotalSubscriptions = 0,
            ActiveSubscriptions = 0
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TenantProductDto>> Update([FromRoute] Guid id, [FromBody] UpdateTenantProductRequestDto request)
    {
        var entity = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null)
            return NotFound();

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest();

            entity.Name = request.Name.Trim();
        }

        if (request.Slug != null)
        {
            if (string.IsNullOrWhiteSpace(request.Slug))
                return BadRequest();

            var slug = request.Slug.Trim();
            var exists = await _dbContext.Products.AsNoTracking().AnyAsync(p => p.Id != id && p.Slug == slug);
            if (exists)
            {
                return BadRequest();
            }

            entity.Slug = slug;
        }

        if (request.Description != null) entity.Description = request.Description;

        if (request.Category != null)
        {
            if (!Enum.TryParse<ETenantProductCategory>(request.Category, ignoreCase: true, out var category))
            {
                return BadRequest();
            }
            entity.Category = category;
        }

        if (request.Version != null)
        {
            entity.Version = request.Version;
        }

        if (request.Status != null)
        {
            if (!Enum.TryParse<EProductStatus>(request.Status, ignoreCase: true, out var status))
            {
                return BadRequest();
            }
            entity.Status = status;
        }

        if (request.ConfigurationSchema != null) entity.ConfigurationSchema = request.ConfigurationSchema;

        if (request.PricingModel != null)
        {
            if (!Enum.TryParse<ETenantProductPricingModel>(request.PricingModel, ignoreCase: true, out var pricing))
            {
                return BadRequest();
            }
            entity.PricingModel = pricing;
        }

        if (request.BasePrice.HasValue) entity.BasePrice = request.BasePrice.Value;
        if (request.SetupFee.HasValue) entity.SetupFee = request.SetupFee.Value;

        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        var total = await _dbContext.Subscriptions.AsNoTracking().CountAsync(s => s.ProductId == id);
        var active = await _dbContext.Subscriptions.AsNoTracking().CountAsync(s => s.ProductId == id && (s.Status == ESubscriptionStatus.Ativo || s.Status == ESubscriptionStatus.Trial));

        return Ok(new TenantProductDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Slug = entity.Slug,
            Description = entity.Description,
            Category = entity.Category.ToString(),
            Version = entity.Version,
            Status = entity.Status.ToString(),
            ConfigurationSchema = entity.ConfigurationSchema,
            PricingModel = entity.PricingModel.ToString(),
            BasePrice = entity.BasePrice,
            SetupFee = entity.SetupFee,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            TotalSubscriptions = total,
            ActiveSubscriptions = active
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete([FromRoute] Guid id)
    {
        var entity = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null)
        {
            return Ok(false);
        }

        _dbContext.Products.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return Ok(true);
    }
}

