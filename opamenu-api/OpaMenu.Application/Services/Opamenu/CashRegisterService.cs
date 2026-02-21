using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.CashRegister;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

public class CashRegisterService(
    ICashRegisterRepository cashRegisterRepository,
    IOrderRepository orderRepository,
    ICurrentUserService currentUserService,
    ILogger<CashRegisterService> logger
    ) : ICashRegisterService
{
    private readonly ICashRegisterRepository _cashRegisterRepository = cashRegisterRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ILogger<CashRegisterService> _logger = logger;

    private (Guid tenantId, Guid userId) GetContext()
    {
        var tenantGuid = _currentUserService.GetTenantGuid();
        if (!tenantGuid.HasValue) throw new InvalidOperationException("Tenant não identificado.");
        if (!Guid.TryParse(_currentUserService.UserId, out var userGuid)) throw new InvalidOperationException("Usuário não identificado.");
        
        return (tenantGuid.Value, userGuid);
    }

    public async Task<ResponseDTO<CashShiftResponseDto>> GetActiveShiftAsync()
    {
        try
        {
            var (tenantId, userId) = GetContext();
            var shift = await _cashRegisterRepository.GetActiveShiftAsync(userId, tenantId);
            if (shift == null) return StaticResponseBuilder<CashShiftResponseDto>.BuildOk(null!);

            return StaticResponseBuilder<CashShiftResponseDto>.BuildOk(MapToDto(shift));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter caixa ativo");
            return StaticResponseBuilder<CashShiftResponseDto>.BuildError("Erro ao obter caixa");
        }
    }

    public async Task<ResponseDTO<CashShiftResponseDto>> OpenShiftAsync(OpenCashShiftRequestDto request)
    {
        try
        {
            var (tenantId, userId) = GetContext();
            var activeShift = await _cashRegisterRepository.GetActiveShiftAsync(userId, tenantId);
            if (activeShift != null)
                return StaticResponseBuilder<CashShiftResponseDto>.BuildError("Já existe um caixa aberto para este usuário.");

            var shift = new CashShiftEntity
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UserId = userId,
                OpenedAt = DateTime.UtcNow,
                OpeningBalance = request.OpeningBalance,
                ExpectedBalance = request.OpeningBalance,
                Status = ECashShiftStatus.Open,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cashRegisterRepository.AddAsync(shift);

            // Registrar movimento de abertura
            await _cashRegisterRepository.AddMovementAsync(new CashMovementEntity
            {
                Id = Guid.NewGuid(),
                ShiftId = shift.Id,
                TenantId = tenantId,
                Type = ECashMovementType.Opening,
                Amount = request.OpeningBalance,
                Description = "Abertura de caixa",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            return StaticResponseBuilder<CashShiftResponseDto>.BuildOk(MapToDto(shift));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao abrir caixa");
            return StaticResponseBuilder<CashShiftResponseDto>.BuildError("Erro ao abrir caixa");
        }
    }

    public async Task<ResponseDTO<CashShiftResponseDto>> CloseShiftAsync(CloseCashShiftRequestDto request)
    {
        try
        {
            var (tenantId, userId) = GetContext();
            var shift = await _cashRegisterRepository.GetActiveShiftAsync(userId, tenantId);
            if (shift == null) return StaticResponseBuilder<CashShiftResponseDto>.BuildError("Não há caixa aberto para fechar.");

            shift.Status = ECashShiftStatus.Closed;
            shift.ClosedAt = DateTime.UtcNow;
            shift.ClosingBalance = request.ClosingBalance;
            shift.UpdatedAt = DateTime.UtcNow;

            await _cashRegisterRepository.UpdateAsync(shift);

            // Registrar movimento de fechamento
            await _cashRegisterRepository.AddMovementAsync(new CashMovementEntity
            {
                Id = Guid.NewGuid(),
                ShiftId = shift.Id,
                TenantId = tenantId,
                Type = ECashMovementType.Closing,
                Amount = request.ClosingBalance,
                Description = "Fechamento de caixa",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            return StaticResponseBuilder<CashShiftResponseDto>.BuildOk(MapToDto(shift));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fechar caixa");
            return StaticResponseBuilder<CashShiftResponseDto>.BuildError("Erro ao fechar caixa");
        }
    }

    public async Task<ResponseDTO<CashMovementResponseDto>> AddMovementAsync(AddCashMovementRequestDto request)
    {
        try
        {
            var (tenantId, userId) = GetContext();
            var shift = await _cashRegisterRepository.GetActiveShiftAsync(userId, tenantId);
            if (shift == null) return StaticResponseBuilder<CashMovementResponseDto>.BuildError("Não há caixa aberto para registrar movimentação.");

            var movement = new CashMovementEntity
            {
                Id = Guid.NewGuid(),
                ShiftId = shift.Id,
                TenantId = tenantId,
                Type = request.Type,
                Amount = request.Amount,
                Description = request.Description,
                PaymentMethod = request.PaymentMethod,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cashRegisterRepository.AddMovementAsync(movement);

            // Atualizar saldo esperado no shift
            if (request.Type == ECashMovementType.Inbound || request.Type == ECashMovementType.OrderPayment)
                shift.ExpectedBalance += request.Amount;
            else if (request.Type == ECashMovementType.Outbound || request.Type == ECashMovementType.Reversed)
                shift.ExpectedBalance -= request.Amount;

            await _cashRegisterRepository.UpdateAsync(shift);

            return StaticResponseBuilder<CashMovementResponseDto>.BuildOk(MapMovementToDto(movement));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar movimentação ao caixa");
            return StaticResponseBuilder<CashMovementResponseDto>.BuildError("Erro ao registrar movimentação");
        }
    }

    public async Task<ResponseDTO<IEnumerable<CashShiftResponseDto>>> GetShiftHistoryAsync(int count = 10)
    {
        try
        {
            var (tenantId, _) = GetContext();
            var history = await _cashRegisterRepository.GetShiftHistoryAsync(tenantId, count);
            return StaticResponseBuilder<IEnumerable<CashShiftResponseDto>>.BuildOk(history.Select(MapToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter histórico de caixa");
            return StaticResponseBuilder<IEnumerable<CashShiftResponseDto>>.BuildError("Erro ao obter histórico");
        }
    }

    private CashShiftResponseDto MapToDto(CashShiftEntity entity)
    {
        return new CashShiftResponseDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            OpenedAt = entity.OpenedAt,
            ClosedAt = entity.ClosedAt,
            OpeningBalance = entity.OpeningBalance,
            ClosingBalance = entity.ClosingBalance,
            ExpectedBalance = entity.ExpectedBalance,
            Status = entity.Status,
            Movements = entity.Movements?.Select(MapMovementToDto).ToList() ?? new()
        };
    }

    private CashMovementResponseDto MapMovementToDto(CashMovementEntity entity)
    {
        return new CashMovementResponseDto
        {
            Id = entity.Id,
            Type = entity.Type,
            Amount = entity.Amount,
            Description = entity.Description ?? string.Empty,
            PaymentMethod = entity.PaymentMethod,
            OrderId = entity.OrderId,
            CreatedAt = entity.CreatedAt
        };
    }
}
