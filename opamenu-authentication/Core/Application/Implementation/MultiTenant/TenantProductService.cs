using Authenticator.API.Core.Application.Interfaces.MultiTenant;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Core.Domain.Api.Commons;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.TenantProduct;
using Authenticator.API.Core.Domain.MultiTenant.TenantProduct.DTOs;
using AutoMapper;

namespace Authenticator.API.Core.Application.Implementation.MultiTenant;

public class TenantProductService(
    ITenantProductRepository productRepository,
    IMapper mapper,
    ILogger<TenantProductService> logger) : ITenantProductService
{
    public async Task<ResponseDTO<TenantProductDTO>> CreateAsync(CreateTenantProductDTO dto)
    {
        try
        {
            var entity = mapper.Map<TenantProductEntity>(dto);

            var created = await productRepository.AddAsync(entity);
            var resultDto = mapper.Map<TenantProductDTO>(created);

            return StaticResponseBuilder<TenantProductDTO>.BuildOk(resultDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar produto: {Name}", dto.Name);
            return StaticResponseBuilder<TenantProductDTO>.BuildError($"Erro interno ao criar produto. {ex.Message}");
        }
    }

    public async Task<ResponseDTO<TenantProductDTO>> UpdateAsync(Guid id, UpdateTenantProductDTO dto)
    {
        try
        {
            var entity = await productRepository.GetByIdAsync(id);
            if (entity == null)
                return StaticResponseBuilder<TenantProductDTO>.BuildError("Produto nÃ£o encontrado.");

            mapper.Map(dto, entity);

            await productRepository.UpdateAsync(entity);
            var resultDto = mapper.Map<TenantProductDTO>(entity);

            return StaticResponseBuilder<TenantProductDTO>.BuildOk(resultDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao atualizar produto: {Id}", id);
            return StaticResponseBuilder<TenantProductDTO>.BuildError($"Erro interno ao atualizar produto. {ex.Message}");
        }
    }

    public async Task<ResponseDTO<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await productRepository.GetByIdAsync(id);
            if (entity == null)
                return StaticResponseBuilder<bool>.BuildError("Produto nÃ£o encontrado.");

            await productRepository.DeleteAsync(entity);

            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao remover produto: {Id}", id);
            return StaticResponseBuilder<bool>.BuildError($"Erro interno ao remover produto. {ex.Message}");
        }
    }

    public async Task<ResponseDTO<TenantProductDTO>> GetByIdAsync(Guid id)
    {
        var entity = await productRepository.GetByIdAsync(id);
        if (entity == null)
            return StaticResponseBuilder<TenantProductDTO>.BuildError("Produto nÃ£o encontrado.");

        var resultDto = mapper.Map<TenantProductDTO>(entity);
        return StaticResponseBuilder<TenantProductDTO>.BuildOk(resultDto);
    }

    public async Task<ResponseDTO<List<TenantProductDTO>>> GetAllAsync()
    {
        try
        {
            var all = await productRepository.GetAllAsync();
            if (all == null)
                return StaticResponseBuilder<List<TenantProductDTO>>.BuildNoContent(new List<TenantProductDTO>());

            var resultDto = mapper.Map<List<TenantProductDTO>>(all);
            return StaticResponseBuilder<List<TenantProductDTO>>.BuildOk(resultDto);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<List<TenantProductDTO>>.BuildError($"Erro interno ao obter produtos.{ex.Message}");
        }

    }

    public async Task<ResponseDTO<List<TenantProductSummaryDTO>>> GetActiveProductsAsync()
    {
        try
        {
            var active = await productRepository.FindAsync(p => p.Status == EProductStatus.Ativo);
            var resultDto = mapper.Map<List<TenantProductSummaryDTO>>(active);
            return StaticResponseBuilder<List<TenantProductSummaryDTO>>.BuildOk(resultDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter produtos ativos.");
            return StaticResponseBuilder<List<TenantProductSummaryDTO>>.BuildError($"Erro interno ao obter produtos ativos. {ex.Message}");

        }
        
    }
}

