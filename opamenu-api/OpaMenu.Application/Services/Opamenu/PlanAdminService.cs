using Microsoft.EntityFrameworkCore;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Domain.DTOs.MultiTenant;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.PlanModule;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Subscription;
using System.Text.Json;

namespace OpaMenu.Application.Services.Opamenu;

public sealed class PlanAdminService(
    IPlanRepository planRepository,
    IPlanModuleRepository planModuleRepository,
    ISubscriptionRepository subscriptionRepository) : IPlanAdminService
{
    private readonly IPlanRepository _planRepository = planRepository;
    private readonly IPlanModuleRepository _planModuleRepository = planModuleRepository;
    private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;

    public async Task<PlanListResponseDto> GetAllAsync(int page, int pageSize, string? name = null, string? status = null)
    {
        var (plans, total) = await _planRepository.GetPagedAsync(page, pageSize, name, status);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        var planIds = plans.Select(p => p.Id).ToList();

        // Agregação de assinaturas (simplificado para o serviço)
        // Nota: Em um cenário real, poderíamos ter um método no repositório de assinaturas para isso
        var subscriptions = await _subscriptionRepository.GetAllAsync(); // Idealmente filtrar por PlanIds
        var subscriptionAgg = subscriptions
            .Where(s => planIds.Contains(s.PlanId))
            .GroupBy(s => s.PlanId)
            .Select(g => new
            {
                PlanId = g.Key,
                Total = g.Count(),
                Active = g.Count(x => x.Status == ESubscriptionStatus.Ativo || x.Status == ESubscriptionStatus.Trial)
            })
            .ToDictionary(x => x.PlanId, x => x);

        var items = plans.Select(p =>
        {
            subscriptionAgg.TryGetValue(p.Id, out var agg);
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
        }).ToList();

        return new PlanListResponseDto
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
        };
    }

    public async Task<ApiResponseDto<PlanDto>> GetByIdAsync(Guid id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        if (plan == null)
        {
            return new ApiResponseDto<PlanDto>
            {
                Succeeded = false,
                Data = new PlanDto(),
                Errors = [new ErrorDto { Message = "Plano não encontrado" }]
            };
        }

        // Simplificado para demonstração, idealmente via repositório
        var subscriptions = await _subscriptionRepository.GetAllAsync();
        var planSubs = subscriptions.Where(s => s.PlanId == id).ToList();
        var total = planSubs.Count;
        var active = planSubs.Count(s => s.Status == ESubscriptionStatus.Ativo || s.Status == ESubscriptionStatus.Trial);
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

        return new ApiResponseDto<PlanDto> { Succeeded = true, Data = dto, Errors = [] };
    }

    public async Task<IEnumerable<object>> GetActiveAsync()
    {
        var plans = await _planRepository.GetAllActiveAsync();
        return plans.Select(p => new
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            BillingCycle = p.BillingCycle.ToString(),
            Features = p.Features,
            IsActive = true
        });
    }

    public async Task<PlanDto> CreateAsync(CreatePlanRequestDto request)
    {
        if (!Enum.TryParse<EBillingCycle>(request.BillingCycle, ignoreCase: true, out var billingCycle))
            throw new ArgumentException("Ciclo de cobrança inválido");

        if (!Enum.TryParse<EPlanStatus>(request.Status, ignoreCase: true, out var status))
            throw new ArgumentException("Status inválido");

        if (await _planRepository.SlugExistsAsync(request.Slug))
            throw new ArgumentException("Slug já existe");

        var entity = new PlanEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = request.Slug.Trim(),
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
            CreatedAt = DateTime.UtcNow
        };

        using var transaction = await _planRepository.BeginTransactionAsync();
        try
        {
            await _planRepository.AddAsync(entity);
            await _planRepository.SaveChangesAsync();

            await SyncPlanModulesAsync(entity.Id, entity.Features);

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return MapToDto(entity);
    }

    public async Task<PlanDto> UpdateAsync(Guid id, UpdatePlanRequestDto request)
    {
        var entity = await _planRepository.GetByIdTrackedAsync(id);
        if (entity == null) throw new KeyNotFoundException("Plano não encontrado");

        if (request.Name != null) entity.Name = request.Name.Trim();
        if (request.Slug != null)
        {
            if (await _planRepository.SlugExistsAsync(request.Slug, id))
                throw new ArgumentException("Slug já existe");
            entity.Slug = request.Slug.Trim();
        }
        if (request.Description != null) entity.Description = request.Description;
        if (request.Price.HasValue) entity.Price = request.Price.Value;
        if (request.BillingCycle != null)
        {
            if (Enum.TryParse<EBillingCycle>(request.BillingCycle, ignoreCase: true, out var bc))
                entity.BillingCycle = bc;
        }
        if (request.MaxUsers.HasValue) entity.MaxUsers = request.MaxUsers.Value;
        if (request.MaxStorageGb.HasValue) entity.MaxStorageGb = request.MaxStorageGb.Value;
        if (request.Features != null) entity.Features = request.Features;
        if (request.Status != null)
        {
            if (Enum.TryParse<EPlanStatus>(request.Status, ignoreCase: true, out var st))
                entity.Status = st;
        }
        if (request.SortOrder.HasValue) entity.SortOrder = request.SortOrder.Value;
        if (request.IsTrial.HasValue) entity.IsTrial = request.IsTrial.Value;
        if (request.TrialPeriodDays.HasValue) entity.TrialPeriodDays = request.TrialPeriodDays.Value;

        entity.UpdatedAt = DateTime.UtcNow;
        
        await _planRepository.UpdateAsync(entity);
        await _planRepository.SaveChangesAsync();

        if (request.Features != null)
        {
            await SyncPlanModulesAsync(entity.Id, entity.Features);
        }

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _planRepository.GetByIdAsync(id);
        if (entity == null) return false;

        await _planRepository.DeleteAsync(entity);
        await _planRepository.SaveChangesAsync();
        return true;
    }

    private async Task SyncPlanModulesAsync(Guid planId, string? featuresJson)
    {
        if (string.IsNullOrWhiteSpace(featuresJson))
        {
            var existing = await _planModuleRepository.GetByPlanIdAsync(planId);
            if (existing.Count > 0)
            {
                await _planModuleRepository.RemoveRangeAsync(existing);
                await _planModuleRepository.SaveChangesAsync();
            }
            return;
        }

        try
        {
            var features = JsonSerializer.Deserialize<PlanFeaturesDto>(featuresJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var moduleIds = features?.ModuleIds ?? new List<Guid>();

            var existingModules = await _planModuleRepository.GetByPlanIdAsync(planId);
            var existingIds = existingModules.Select(m => m.ModuleId).ToList();

            var toRemove = existingModules.Where(m => !moduleIds.Contains(m.ModuleId)).ToList();
            var toAdd = moduleIds
                .Where(id => id != Guid.Empty && !existingIds.Contains(id))
                .Select(id => new PlanModuleEntity
                {
                    Id = Guid.NewGuid(),
                    PlanId = planId,
                    ModuleId = id,
                    IsIncluded = true,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            if (toRemove.Count > 0) await _planModuleRepository.RemoveRangeAsync(toRemove);
            if (toAdd.Count > 0) await _planModuleRepository.AddRangeAsync(toAdd);

            if (toRemove.Count > 0 || toAdd.Count > 0)
            {
                await _planModuleRepository.SaveChangesAsync();
            }
        }
        catch (JsonException) { }
    }

    private static PlanDto MapToDto(PlanEntity entity)
    {
        return new PlanDto
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
            IsTrial = entity.IsTrial,
            TrialPeriodDays = entity.TrialPeriodDays
        };
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

    private sealed class PlanFeaturesDto
    {
        public List<Guid> ModuleIds { get; set; } = new();
    }
}
