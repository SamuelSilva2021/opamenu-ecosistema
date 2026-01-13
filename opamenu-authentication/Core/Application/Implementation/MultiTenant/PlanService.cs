using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.Api.Commons;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Plan;
using Authenticator.API.Core.Domain.MultiTenant.Plan.DTOs;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.MultiTenant;

public class PlanService(
    IPlanRepository planRepository,
    IMapper mapper,
    ILogger<PlanService> logger) : IPlanService
{
    public async Task<ResponseDTO<PlanDTO>> CreateAsync(CreatePlanDTO dto)
    {
        try
        {
            var existingSlug = await planRepository.FirstOrDefaultAsync(p => p.Slug == dto.Slug);
            if (existingSlug != null)
                return StaticResponseBuilder<PlanDTO>.BuildError("JÃ¡ existe um plano com este slug.");

            var entity = mapper.Map<PlanEntity>(dto);

            var created = await planRepository.AddAsync(entity);

            var resultDto = mapper.Map<PlanDTO>(created);

            return StaticResponseBuilder<PlanDTO>.BuildOk(resultDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar plano: {Slug}", dto.Slug);
            return StaticResponseBuilder<PlanDTO>.BuildError("Erro interno ao criar plano.");
        }
    }

    public async Task<ResponseDTO<PlanDTO>> UpdateAsync(Guid id, UpdatePlanDTO dto)
    {
        try
        {
            var entity = await planRepository.GetByIdAsync(id);
            if (entity == null)
                return StaticResponseBuilder<PlanDTO>.BuildError("Plano nÃ£o encontrado.");

            if (!string.IsNullOrEmpty(dto.Slug) && dto.Slug != entity.Slug)
            {
                var existingSlug = await planRepository.FirstOrDefaultAsync(p => p.Slug == dto.Slug && p.Id != id);
                if (existingSlug != null)
                    return StaticResponseBuilder<PlanDTO>.BuildError("JÃ¡ existe um plano com este slug.");
            }

            mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            await planRepository.UpdateAsync(entity);
            var resultDto = mapper.Map<PlanDTO>(entity);

            return StaticResponseBuilder<PlanDTO>.BuildOk(resultDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao atualizar plano: {Id}", id);
            return StaticResponseBuilder<PlanDTO>.BuildError("Erro interno ao atualizar plano.");
        }
    }

    public async Task<ResponseDTO<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await planRepository.GetByIdAsync(id);
            if (entity == null)
                return StaticResponseBuilder<bool>.BuildError("Plano nÃ£o encontrado.");

            // Soft delete ou Hard delete? Geralmente soft delete se tiver relacionamentos.
            // Aqui faremos hard delete por enquanto, mas o ideal seria verificar assinaturas ativas.
            await planRepository.DeleteAsync(entity);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao remover plano: {Id}", id);
            return StaticResponseBuilder<bool>.BuildError("Erro interno ao remover plano.");
        }
    }

    public async Task<ResponseDTO<PlanDTO>> GetByIdAsync(Guid id)
    {
        var entity = await planRepository.GetByIdAsync(id);
        if (entity == null)
            return StaticResponseBuilder<PlanDTO>.BuildError("Plano nÃ£o encontrado.");

        var planDto = mapper.Map<PlanDTO>(entity);

        return StaticResponseBuilder<PlanDTO>.BuildOk(planDto);
    }

    public async Task<ResponseDTO<PlanDTO>> GetBySlugAsync(string slug)
    {
        var entity = await planRepository.FirstOrDefaultAsync(p => p.Slug == slug);
        if (entity == null)
            return StaticResponseBuilder<PlanDTO>.BuildError("Plano nÃ£o encontrado.");

        var planDto = mapper.Map<PlanDTO>(entity);

        return StaticResponseBuilder<PlanDTO>.BuildOk(planDto);
    }

    public async Task<ResponseDTO<PagedResponseDTO<PlanDTO>>> GetAllAsync(PlanFilterDTO filter)
    {
        try
        {
            var allPlans = await planRepository.GetAllAsync();
            var query = allPlans.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(p => p.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));

            var total = query.Count();
            var items = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var dtos = mapper.Map<List<PlanDTO>>(items);

            return StaticResponseBuilder<PagedResponseDTO<PlanDTO>>.BuildOk(
                new PagedResponseDTO<PlanDTO>
                {
                    Items = dtos,
                    Total = total,
                    Page = filter.Page,
                    Limit = filter.PageSize,
                    CurrentPage = filter.Page,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((double)total / filter.PageSize),
                    Succeeded = true,
                    Code = 200
                }
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao listar planos.");
            return StaticResponseBuilder<PagedResponseDTO<PlanDTO>>.BuildError("Erro ao listar planos.");
        }
    }

    public async Task<ResponseDTO<List<PlanSummaryDTO>>> GetActivePlansAsync()
    {
        try
        {
            var activePlans = await planRepository.FindAsync(p => p.Status == EPlanStatus.Ativo);
            var dtos = mapper.Map<List<PlanSummaryDTO>>(activePlans);
            return StaticResponseBuilder<List<PlanSummaryDTO>>.BuildOk(dtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter planos ativos.");
            return StaticResponseBuilder<List<PlanSummaryDTO>>.BuildError("Erro ao obter planos ativos.");
        }
    }
}

