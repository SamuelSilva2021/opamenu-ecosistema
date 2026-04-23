using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.MultiTenant;
using OpaMenu.Infrastructure.Shared.Data.Context.MultTenant;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;

namespace OpaMenu.Web.UserEntry.MultiTenant;

[ApiController]
[Route("api/plans")]
[Authorize(Roles = "SUPER_ADMIN")]
public sealed class PlansController(MultiTenantDbContext dbContext) : ControllerBase
{
    private readonly MultiTenantDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<PlanListResponseDto>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? name = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] string? status = null)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var query = _dbContext.Plans.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var n = name.Trim();
            query = query.Where(p => p.Name.Contains(n) || p.Slug.Contains(n));
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => (p.Status == EPlanStatus.Ativo) == isActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (Enum.TryParse<EPlanStatus>(status, ignoreCase: true, out var st))
            {
                query = query.Where(p => p.Status == st);
            }
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        var planIds = await query
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => p.Id)
            .ToListAsync();

        var plans = await _dbContext.Plans.AsNoTracking()
            .Where(p => planIds.Contains(p.Id))
            .ToListAsync();

        var subscriptionAgg = await _dbContext.Subscriptions.AsNoTracking()
            .Where(s => planIds.Contains(s.PlanId))
            .GroupBy(s => s.PlanId)
            .Select(g => new
            {
                PlanId = g.Key,
                Total = g.Count(),
                Active = g.Count(x => x.Status == ESubscriptionStatus.Ativo || x.Status == ESubscriptionStatus.Trial)
            })
            .ToListAsync();

        var aggByPlan = subscriptionAgg.ToDictionary(x => x.PlanId, x => x);

        var items = plans
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Name)
            .Select(p =>
            {
                aggByPlan.TryGetValue(p.Id, out var agg);
                var activeCount = agg?.Active ?? 0;
                var mrr = activeCount * p.Price * BillingCycleMonthlyFactor(p.BillingCycle);
                return new PlanDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug,
                    Description = p.Description,
                    Price = p.Price,
                    BillingCycle = p.BillingCycle.ToString(),
                    MaxUsers = p.MaxUsers,
                    MaxStorageGb = p.MaxStorageGb,
                    Features = p.Features,
                    Status = p.Status.ToString(),
                    SortOrder = p.SortOrder,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    TotalSubscriptions = agg?.Total ?? 0,
                    ActiveSubscriptions = activeCount,
                    MonthlyRecurringRevenue = decimal.Round(mrr, 2),
                    IsTrial = p.IsTrial,
                    TrialPeriodDays = p.TrialPeriodDays
                };
            })
            .ToList();

        return Ok(new PlanListResponseDto
        {
            Items = items,
            Page = page,
            Limit = pageSize,
            Total = total,
            TotalPages = totalPages,
            Succeeded = true,
            Code = 200,
            CurrentPage = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<PlanDto>>> GetById([FromRoute] Guid id)
    {
        var plan = await _dbContext.Plans.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (plan == null)
        {
            return NotFound(new ApiResponseDto<PlanDto>
            {
                Succeeded = false,
                Data = new PlanDto(),
                Errors = [new ErrorDto { Message = "Plano não encontrado" }]
            });
        }

        var total = await _dbContext.Subscriptions.AsNoTracking().CountAsync(s => s.PlanId == id);
        var active = await _dbContext.Subscriptions.AsNoTracking().CountAsync(s => s.PlanId == id && (s.Status == ESubscriptionStatus.Ativo || s.Status == ESubscriptionStatus.Trial));
        var mrr = active * plan.Price * BillingCycleMonthlyFactor(plan.BillingCycle);

        var dto = new PlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Slug = plan.Slug,
            Description = plan.Description,
            Price = plan.Price,
            BillingCycle = plan.BillingCycle.ToString(),
            MaxUsers = plan.MaxUsers,
            MaxStorageGb = plan.MaxStorageGb,
            Features = plan.Features,
            Status = plan.Status.ToString(),
            SortOrder = plan.SortOrder,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt,
            TotalSubscriptions = total,
            ActiveSubscriptions = active,
            MonthlyRecurringRevenue = decimal.Round(mrr, 2),
            IsTrial = plan.IsTrial,
            TrialPeriodDays = plan.TrialPeriodDays
        };

        return Ok(new ApiResponseDto<PlanDto> { Succeeded = true, Data = dto, Errors = [] });
    }

    [HttpPost]
    public async Task<ActionResult<PlanDto>> Create([FromBody] CreatePlanRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Slug))
        {
            return BadRequest();
        }

        if (!Enum.TryParse<EBillingCycle>(request.BillingCycle, ignoreCase: true, out var billingCycle))
        {
            return BadRequest();
        }

        if (!Enum.TryParse<EPlanStatus>(request.Status, ignoreCase: true, out var status))
        {
            return BadRequest();
        }

        var slug = request.Slug.Trim();
        var existsSlug = await _dbContext.Plans.AsNoTracking().AnyAsync(p => p.Slug == slug);
        if (existsSlug)
        {
            return BadRequest();
        }

        var entity = new PlanEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = slug,
            Description = request.Description,
            Price = request.Price,
            BillingCycle = billingCycle,
            MaxUsers = request.MaxUsers,
            MaxStorageGb = request.MaxStorageGb,
            Features = request.Features,
            Status = status,
            IsTrial = request.IsTrial ?? false,
            TrialPeriodDays = request.TrialPeriodDays ?? 0,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _dbContext.Plans.Add(entity);
        await _dbContext.SaveChangesAsync();

        return Ok(new PlanDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Slug = entity.Slug,
            Description = entity.Description,
            Price = entity.Price,
            BillingCycle = entity.BillingCycle.ToString(),
            MaxUsers = entity.MaxUsers,
            MaxStorageGb = entity.MaxStorageGb,
            Features = entity.Features,
            Status = entity.Status.ToString(),
            SortOrder = entity.SortOrder,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            TotalSubscriptions = 0,
            ActiveSubscriptions = 0,
            MonthlyRecurringRevenue = 0,
            IsTrial = entity.IsTrial,
            TrialPeriodDays = entity.TrialPeriodDays
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PlanDto>> Update([FromRoute] Guid id, [FromBody] UpdatePlanRequestDto request)
    {
        var entity = await _dbContext.Plans.FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null)
        {
            return NotFound();
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest();
            }

            entity.Name = request.Name.Trim();
        }

        if (request.Slug != null)
        {
            if (string.IsNullOrWhiteSpace(request.Slug))
            {
                return BadRequest();
            }

            var slug = request.Slug.Trim();
            var existsSlug = await _dbContext.Plans.AsNoTracking().AnyAsync(p => p.Id != id && p.Slug == slug);
            if (existsSlug)
            {
                return BadRequest();
            }

            entity.Slug = slug;
        }

        if (request.Description != null)
        {
            entity.Description = request.Description;
        }

        if (request.Price.HasValue)
        {
            entity.Price = request.Price.Value;
        }

        if (request.BillingCycle != null)
        {
            if (!Enum.TryParse<EBillingCycle>(request.BillingCycle, ignoreCase: true, out var billingCycle))
            {
                return BadRequest();
            }

            entity.BillingCycle = billingCycle;
        }

        if (request.MaxUsers.HasValue) entity.MaxUsers = request.MaxUsers.Value;
        if (request.MaxStorageGb.HasValue) entity.MaxStorageGb = request.MaxStorageGb.Value;
        if (request.Features != null) entity.Features = request.Features;

        if (request.Status != null)
        {
            if (!Enum.TryParse<EPlanStatus>(request.Status, ignoreCase: true, out var status))
            {
                return BadRequest();
            }
            entity.Status = status;
        }

        if (request.SortOrder.HasValue) entity.SortOrder = request.SortOrder.Value;
        if (request.IsTrial.HasValue) entity.IsTrial = request.IsTrial.Value;
        if (request.TrialPeriodDays.HasValue) entity.TrialPeriodDays = request.TrialPeriodDays.Value;

        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        var total = await _dbContext.Subscriptions.AsNoTracking().CountAsync(s => s.PlanId == id);
        var active = await _dbContext.Subscriptions.AsNoTracking().CountAsync(s => s.PlanId == id && (s.Status == ESubscriptionStatus.Ativo || s.Status == ESubscriptionStatus.Trial));
        var mrr = active * entity.Price * BillingCycleMonthlyFactor(entity.BillingCycle);

        return Ok(new PlanDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Slug = entity.Slug,
            Description = entity.Description,
            Price = entity.Price,
            BillingCycle = entity.BillingCycle.ToString(),
            MaxUsers = entity.MaxUsers,
            MaxStorageGb = entity.MaxStorageGb,
            Features = entity.Features,
            Status = entity.Status.ToString(),
            SortOrder = entity.SortOrder,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            TotalSubscriptions = total,
            ActiveSubscriptions = active,
            MonthlyRecurringRevenue = decimal.Round(mrr, 2),
            IsTrial = entity.IsTrial,
            TrialPeriodDays = entity.TrialPeriodDays
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete([FromRoute] Guid id)
    {
        var entity = await _dbContext.Plans.FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null)
        {
            return Ok(false);
        }

        _dbContext.Plans.Remove(entity);
        await _dbContext.SaveChangesAsync();
        return Ok(true);
    }

    private static decimal BillingCycleMonthlyFactor(EBillingCycle cycle)
    {
        return cycle switch
        {
            EBillingCycle.Mensal => 1m,
            EBillingCycle.Anual => 1m / 12m,
            EBillingCycle.Semestral => 1m / 6m,
            EBillingCycle.Semanal => 52m / 12m,
            EBillingCycle.Diario => 365m / 12m,
            _ => 1m
        };
    }
}

