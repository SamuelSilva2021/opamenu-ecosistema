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

    public async Task<ResponseDTO<CashShiftSummaryResponseDto>> GetActiveShiftSummaryAsync()
    {
        try
        {
            var (tenantId, userId) = GetContext();
            var shift = await _cashRegisterRepository.GetActiveShiftAsync(userId, tenantId);
            if (shift == null) return StaticResponseBuilder<CashShiftSummaryResponseDto>.BuildOk(null!);

            return StaticResponseBuilder<CashShiftSummaryResponseDto>.BuildOk(BuildSummary(shift));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter resumo do caixa ativo");
            return StaticResponseBuilder<CashShiftSummaryResponseDto>.BuildError("Erro ao obter resumo do caixa");
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

            var expectedCashBalance = shift.ExpectedBalance;
            var discrepancy = request.ClosingBalance - expectedCashBalance;
            var discrepancyRequiresJustification = Math.Abs(discrepancy) >= 0.01m;
            var justification = request.DiscrepancyJustification?.Trim();

            if (discrepancyRequiresJustification && string.IsNullOrWhiteSpace(justification))
                return StaticResponseBuilder<CashShiftResponseDto>.BuildError("Informe uma justificativa para a diferença no fechamento.");

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
                Description = BuildClosingDescription(expectedCashBalance, discrepancy, justification),
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

    public async Task<ResponseDTO<CashShiftCloseSummaryResponseDto>> CloseShiftWithSummaryAsync(CloseCashShiftRequestDto request)
    {
        try
        {
            var (tenantId, userId) = GetContext();
            var shift = await _cashRegisterRepository.GetActiveShiftAsync(userId, tenantId);
            if (shift == null) return StaticResponseBuilder<CashShiftCloseSummaryResponseDto>.BuildError("Não há caixa aberto para fechar.");

            var expectedCashBalance = shift.ExpectedBalance;
            var discrepancy = request.ClosingBalance - expectedCashBalance;
            var discrepancyRequiresJustification = Math.Abs(discrepancy) >= 0.01m;
            var justification = request.DiscrepancyJustification?.Trim();

            if (discrepancyRequiresJustification && string.IsNullOrWhiteSpace(justification))
                return StaticResponseBuilder<CashShiftCloseSummaryResponseDto>.BuildError("Informe uma justificativa para a diferença no fechamento.");

            shift.Status = ECashShiftStatus.Closed;
            shift.ClosedAt = DateTime.UtcNow;
            shift.ClosingBalance = request.ClosingBalance;
            shift.UpdatedAt = DateTime.UtcNow;

            await _cashRegisterRepository.UpdateAsync(shift);

            await _cashRegisterRepository.AddMovementAsync(new CashMovementEntity
            {
                Id = Guid.NewGuid(),
                ShiftId = shift.Id,
                TenantId = tenantId,
                Type = ECashMovementType.Closing,
                Amount = request.ClosingBalance,
                Description = BuildClosingDescription(expectedCashBalance, discrepancy, justification),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            var summary = BuildSummary(shift);
            var result = new CashShiftCloseSummaryResponseDto
            {
                Shift = summary.Shift,
                TotalSales = summary.TotalSales,
                TotalInflows = summary.TotalInflows,
                TotalOutflows = summary.TotalOutflows,
                SalesByPaymentMethod = summary.SalesByPaymentMethod,
                ClosingBalance = request.ClosingBalance,
                ExpectedCashBalance = expectedCashBalance,
                Discrepancy = discrepancy,
                DiscrepancyJustification = justification
            };

            return StaticResponseBuilder<CashShiftCloseSummaryResponseDto>.BuildOk(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fechar caixa (resumo)");
            return StaticResponseBuilder<CashShiftCloseSummaryResponseDto>.BuildError("Erro ao fechar caixa");
        }
    }

    public async Task<ResponseDTO<CashMovementResponseDto>> AddMovementAsync(AddCashMovementRequestDto request)
    {
        try
        {
            var (tenantId, userId) = GetContext();
            var shift = await _cashRegisterRepository.GetActiveShiftAsync(userId, tenantId);
            if (shift == null) return StaticResponseBuilder<CashMovementResponseDto>.BuildError("Não há caixa aberto para registrar movimentação.");

            if (request.Amount <= 0m)
                return StaticResponseBuilder<CashMovementResponseDto>.BuildError("Informe um valor maior que zero.");

            if (request.Type is ECashMovementType.Opening or ECashMovementType.Closing)
                return StaticResponseBuilder<CashMovementResponseDto>.BuildError("Tipo de movimentação inválido.");

            if (request.Type is ECashMovementType.OrderPayment or ECashMovementType.Reversed)
            {
                if (!request.OrderId.HasValue)
                    return StaticResponseBuilder<CashMovementResponseDto>.BuildError("OrderId é obrigatório para este tipo de movimentação.");

                if (!request.PaymentMethod.HasValue)
                    return StaticResponseBuilder<CashMovementResponseDto>.BuildError("PaymentMethod é obrigatório para este tipo de movimentação.");
            }

            if (request.Type == ECashMovementType.OrderPayment)
            {
                var orderId = request.OrderId!.Value;
                var existing = await _cashRegisterRepository.GetMovementByOrderAsync(tenantId, shift.Id, orderId, ECashMovementType.OrderPayment);
                if (existing != null)
                    return StaticResponseBuilder<CashMovementResponseDto>.BuildOk(MapMovementToDto(existing));

                var order = await _orderRepository.GetByIdAsync(orderId, tenantId);
                if (order == null)
                    return StaticResponseBuilder<CashMovementResponseDto>.BuildError("Pedido não encontrado para registrar pagamento.");

                if (Math.Abs(order.Total - request.Amount) >= 0.01m)
                    return StaticResponseBuilder<CashMovementResponseDto>.BuildError("Valor do pagamento difere do total do pedido.");

                var paymentMovement = new CashMovementEntity
                {
                    Id = Guid.NewGuid(),
                    ShiftId = shift.Id,
                    TenantId = tenantId,
                    Type = ECashMovementType.OrderPayment,
                    Amount = order.Total,
                    Description = $"Pagamento do pedido #{order.OrderNumber}",
                    PaymentMethod = request.PaymentMethod,
                    OrderId = orderId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _cashRegisterRepository.AddMovementAsync(paymentMovement);

                if (request.PaymentMethod == EPaymentMethod.Cash)
                {
                    shift.ExpectedBalance += order.Total;
                    await _cashRegisterRepository.UpdateAsync(shift);
                }

                return StaticResponseBuilder<CashMovementResponseDto>.BuildOk(MapMovementToDto(paymentMovement));
            }

            if (request.Type == ECashMovementType.Reversed)
            {
                var orderId = request.OrderId!.Value;
                var reversedExisting = await _cashRegisterRepository.GetMovementByOrderAsync(tenantId, shift.Id, orderId, ECashMovementType.Reversed);
                if (reversedExisting != null)
                    return StaticResponseBuilder<CashMovementResponseDto>.BuildOk(MapMovementToDto(reversedExisting));

                var originalPayment = await _cashRegisterRepository.GetMovementByOrderAsync(tenantId, shift.Id, orderId, ECashMovementType.OrderPayment);
                if (originalPayment == null)
                    return StaticResponseBuilder<CashMovementResponseDto>.BuildError("Não existe pagamento registrado no caixa para estornar.");

                if (Math.Abs(originalPayment.Amount - request.Amount) >= 0.01m)
                    return StaticResponseBuilder<CashMovementResponseDto>.BuildError("Valor do estorno difere do pagamento registrado.");

                var reverseMovement = new CashMovementEntity
                {
                    Id = Guid.NewGuid(),
                    ShiftId = shift.Id,
                    TenantId = tenantId,
                    Type = ECashMovementType.Reversed,
                    Amount = originalPayment.Amount,
                    Description = $"Estorno do pedido #{request.OrderId}",
                    PaymentMethod = request.PaymentMethod,
                    OrderId = orderId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _cashRegisterRepository.AddMovementAsync(reverseMovement);

                if (request.PaymentMethod == EPaymentMethod.Cash)
                {
                    shift.ExpectedBalance -= originalPayment.Amount;
                    await _cashRegisterRepository.UpdateAsync(shift);
                }

                return StaticResponseBuilder<CashMovementResponseDto>.BuildOk(MapMovementToDto(reverseMovement));
            }

            var movement = new CashMovementEntity
            {
                Id = Guid.NewGuid(),
                ShiftId = shift.Id,
                TenantId = tenantId,
                Type = request.Type,
                Amount = request.Amount,
                Description = request.Description,
                PaymentMethod = request.PaymentMethod,
                OrderId = request.OrderId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cashRegisterRepository.AddMovementAsync(movement);

            // Atualizar saldo esperado no shift
            if (AffectsCashDrawer(request))
            {
                if (request.Type == ECashMovementType.Inbound || request.Type == ECashMovementType.OrderPayment)
                    shift.ExpectedBalance += request.Amount;
                else if (request.Type == ECashMovementType.Outbound || request.Type == ECashMovementType.Reversed)
                    shift.ExpectedBalance -= request.Amount;
            }

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

    public async Task<ResponseDTO<CashRegisterReportDto>> GetReportAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var (tenantId, _) = GetContext();
            var shifts = await _cashRegisterRepository.GetShiftsByPeriodAsync(tenantId, startDate, endDate);
            
            var report = new CashRegisterReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalGrossSales = 0,
                TotalNetSales = 0,
                TotalInflows = 0,
                TotalOutflows = 0,
                TotalDiscrepancy = 0
            };

            var paymentMethodTotals = new Dictionary<EPaymentMethod, (decimal total, int count)>();

            foreach (var shift in shifts)
            {
                if (shift.Status == ECashShiftStatus.Closed)
                {
                    report.TotalDiscrepancy += (shift.ClosingBalance ?? 0) - shift.ExpectedBalance;
                }

                foreach (var movement in shift.Movements)
                {
                    switch (movement.Type)
                    {
                        case ECashMovementType.OrderPayment:
                            report.TotalGrossSales += movement.Amount;
                            report.TotalNetSales += movement.Amount;
                            
                            if (movement.PaymentMethod.HasValue)
                            {
                                var method = movement.PaymentMethod.Value;
                                if (!paymentMethodTotals.ContainsKey(method))
                                    paymentMethodTotals[method] = (0, 0);
                                
                                var current = paymentMethodTotals[method];
                                paymentMethodTotals[method] = (current.total + movement.Amount, current.count + 1);
                            }
                            break;
                            
                        case ECashMovementType.Inbound:
                        case ECashMovementType.Opening:
                            report.TotalInflows += movement.Amount;
                            break;
                            
                        case ECashMovementType.Outbound:
                        case ECashMovementType.Closing:
                            report.TotalOutflows += movement.Amount;
                            break;
                            
                        case ECashMovementType.Reversed:
                            report.TotalGrossSales -= movement.Amount;
                            report.TotalNetSales -= movement.Amount;
                            break;
                    }
                }
            }

            report.SalesByPaymentMethod = paymentMethodTotals.Select(p => new PaymentMethodSummaryDto
            {
                PaymentMethod = p.Key,
                PaymentMethodName = p.Key.ToString(),
                TotalAmount = p.Value.total,
                Count = p.Value.count
            }).ToList();

            return StaticResponseBuilder<CashRegisterReportDto>.BuildOk(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório de caixa");
            return StaticResponseBuilder<CashRegisterReportDto>.BuildError("Erro ao gerar relatório");
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

    private CashShiftSummaryResponseDto BuildSummary(CashShiftEntity shift)
    {
        var totalsByMethod = new Dictionary<EPaymentMethod, (decimal total, int count)>();
        decimal totalSales = 0m;
        decimal totalInflows = 0m;
        decimal totalOutflows = 0m;

        foreach (var movement in shift.Movements ?? Array.Empty<CashMovementEntity>())
        {
            switch (movement.Type)
            {
                case ECashMovementType.OrderPayment:
                    totalSales += movement.Amount;
                    if (movement.PaymentMethod.HasValue)
                    {
                        var method = movement.PaymentMethod.Value;
                        if (!totalsByMethod.TryGetValue(method, out var current))
                            current = (0m, 0);

                        totalsByMethod[method] = (current.total + movement.Amount, current.count + 1);
                    }
                    break;
                case ECashMovementType.Inbound:
                    totalInflows += movement.Amount;
                    break;
                case ECashMovementType.Outbound:
                    totalOutflows += movement.Amount;
                    break;
            }
        }

        return new CashShiftSummaryResponseDto
        {
            Shift = MapToDto(shift),
            TotalSales = totalSales,
            TotalInflows = totalInflows,
            TotalOutflows = totalOutflows,
            SalesByPaymentMethod = totalsByMethod.Select(p => new PaymentMethodSummaryDto
            {
                PaymentMethod = p.Key,
                PaymentMethodName = GetPaymentMethodDisplayName(p.Key),
                TotalAmount = p.Value.total,
                Count = p.Value.count
            }).ToList()
        };
    }

    private static string GetPaymentMethodDisplayName(EPaymentMethod method)
    {
        return method switch
        {
            EPaymentMethod.Cash => "Dinheiro",
            EPaymentMethod.Pix => "Pix",
            EPaymentMethod.CreditCard => "Cartão de Crédito",
            EPaymentMethod.DebitCard => "Cartão de Débito",
            EPaymentMethod.BankTransfer => "Transferência",
            EPaymentMethod.Ticket => "Ticket",
            _ => method.ToString()
        };
    }

    private static bool AffectsCashDrawer(AddCashMovementRequestDto request)
    {
        return request.Type switch
        {
            ECashMovementType.Inbound => true,
            ECashMovementType.Outbound => true,
            ECashMovementType.OrderPayment => request.PaymentMethod == EPaymentMethod.Cash,
            ECashMovementType.Reversed => request.PaymentMethod == EPaymentMethod.Cash,
            _ => false
        };
    }

    private static string BuildClosingDescription(decimal expectedCashBalance, decimal discrepancy, string? justification)
    {
        if (string.IsNullOrWhiteSpace(justification))
            return $"Fechamento de caixa | Esperado: {expectedCashBalance:0.00} | Diferença: {discrepancy:0.00}";

        return $"Fechamento de caixa | Esperado: {expectedCashBalance:0.00} | Diferença: {discrepancy:0.00} | Justificativa: {justification}";
    }
}
