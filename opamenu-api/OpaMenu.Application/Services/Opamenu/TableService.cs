using AutoMapper;
using Microsoft.Extensions.Logging;
using OpaMenu.Domain.DTOs.Table;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly IUrlBuilderService _urlBuilderService;
    private readonly ILogger<TableService> _logger;

    public TableService(
        ITableRepository tableRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        IUrlBuilderService urlBuilderService,
        ILogger<TableService> logger)
    {
        _tableRepository = tableRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _urlBuilderService = urlBuilderService;
        _logger = logger;
    }

    public async Task<PagedResponseDTO<TableResponseDto>> GetPagedAsync(int pageNumber, int pageSize)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<TableResponseDto>.BuildPagedOk(Enumerable.Empty<TableResponseDto>(), 0, pageNumber, pageSize);

            var tables = await _tableRepository.GetPagedByTenantIdAsync(tenantId.Value, pageNumber, pageSize);
            var total = await _tableRepository.CountByTenantIdAsync(tenantId.Value);

            var dtos = _mapper.Map<IEnumerable<TableResponseDto>>(tables);
            return StaticResponseBuilder<TableResponseDto>.BuildPagedOk(dtos, total, pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter mesas paginadas");
            return new PagedResponseDTO<TableResponseDto>
            {
                Succeeded = false,
                Code = 500,
                Errors = new List<ErrorDTO> { new ErrorDTO { Message = "Erro interno do servidor", Code = "ERROR" } }
            };
        }
    }

    public async Task<ResponseDTO<TableResponseDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<TableResponseDto>.BuildError("Tenant nÃ£o identificado");

            var table = await _tableRepository.GetByIdAsync(id, tenantId.Value);
            if (table == null)
                return StaticResponseBuilder<TableResponseDto>.BuildError("Mesa nÃ£o encontrada");

            return StaticResponseBuilder<TableResponseDto>.BuildOk(_mapper.Map<TableResponseDto>(table));
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<TableResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<TableResponseDto>> CreateAsync(CreateTableRequestDto dto)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<TableResponseDto>.BuildError("Tenant nÃ£o identificado");

            if (await _tableRepository.ExistsByNameAsync(dto.Name, tenantId.Value))
                return StaticResponseBuilder<TableResponseDto>.BuildError("JÃ¡ existe uma mesa com este nome");

            var table = _mapper.Map<TableEntity>(dto);
            table.TenantId = tenantId.Value;
            
            await _tableRepository.AddAsync(table);

            return StaticResponseBuilder<TableResponseDto>.BuildOk(_mapper.Map<TableResponseDto>(table));
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<TableResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<TableResponseDto>> UpdateAsync(Guid id, UpdateTableRequestDto dto)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<TableResponseDto>.BuildError("Tenant nÃ£o identificado");

            var table = await _tableRepository.GetByIdAsync(id, tenantId.Value);
            if (table == null)
                return StaticResponseBuilder<TableResponseDto>.BuildError("Mesa nÃ£o encontrada");

            if (dto.Name != null)
            {
                var existingWithName = await _tableRepository.GetByNameAsync(dto.Name, tenantId.Value);
                if (existingWithName != null && existingWithName.Id != id)
                    return StaticResponseBuilder<TableResponseDto>.BuildError("JÃ¡ existe uma mesa com este nome");
            }

            _mapper.Map(dto, table);
            await _tableRepository.UpdateAsync(table);

            return StaticResponseBuilder<TableResponseDto>.BuildOk(_mapper.Map<TableResponseDto>(table));
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<TableResponseDto>.BuildErrorResponse(ex);
        }
    }

    public async Task<ResponseDTO<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<bool>.BuildError("Tenant nÃ£o identificado");

            var table = await _tableRepository.GetByIdAsync(id, tenantId.Value);
            if (table == null)
                return StaticResponseBuilder<bool>.BuildError("Mesa nÃ£o encontrada");
            
            await _tableRepository.DeleteAsync(table);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
             return StaticResponseBuilder<bool>.BuildErrorResponse(ex);
        }
    }
    
    public async Task<ResponseDTO<string>> GenerateQrCodeAsync(Guid id)
    {
        try
        {
             var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null) return StaticResponseBuilder<string>.BuildError("Tenant nÃ£o identificado");

            var table = await _tableRepository.GetByIdAsync(id, tenantId.Value);
            if (table == null)
                return StaticResponseBuilder<string>.BuildError("Mesa nÃ£o encontrada");

            // Generate QR Code URL using UrlBuilderService
            var baseUrl = _urlBuilderService.GetBaseUrl();
            var qrCodeUrl = $"{baseUrl}/menu/{tenantId}/{table.Id}";
            
            table.QrCodeUrl = qrCodeUrl;
            await _tableRepository.UpdateAsync(table);

            return StaticResponseBuilder<string>.BuildOk(qrCodeUrl);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<string>.BuildErrorResponse(ex);
        }
    }
}

