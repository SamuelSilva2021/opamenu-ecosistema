using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Loyalty;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services;

public class LoyaltyService(
    ILoyaltyProgramRepository loyaltyProgramRepository,
    ICustomerLoyaltyRepository customerLoyaltyRepository,
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    ILogger<LoyaltyService> logger
    ) : ILoyaltyService
{
    private readonly ILoyaltyProgramRepository _loyaltyProgramRepository = loyaltyProgramRepository;
    private readonly ICustomerLoyaltyRepository _customerLoyaltyRepository = customerLoyaltyRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly ILogger<LoyaltyService> _logger = logger;

    public async Task<ResponseDTO<LoyaltyProgramDto>> GetProgramAsync(Guid tenantId)
    {
        try
        {
            var program = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);
            if (program == null)
                return StaticResponseBuilder<LoyaltyProgramDto>.BuildOk(null!);

            return StaticResponseBuilder<LoyaltyProgramDto>.BuildOk(new LoyaltyProgramDto
            {
                Id = program.Id,
                Name = program.Name,
                Description = program.Description,
                PointsPerCurrency = program.PointsPerCurrency,
                CurrencyValue = program.CurrencyValue,
                MinOrderValue = program.MinOrderValue,
                PointsValidityDays = program.PointsValidityDays,
                IsActive = program.IsActive
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter programa de fidelidade");
            return StaticResponseBuilder<LoyaltyProgramDto>.BuildError("Erro ao obter programa");
        }
    }

    public async Task<ResponseDTO<LoyaltyProgramDto>> UpsertProgramAsync(Guid tenantId, CreateLoyaltyProgramDto dto)
    {
        try
        {
            var program = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);

            if (program == null)
            {
                program = new LoyaltyProgramEntity
                {
                    TenantId = tenantId,
                    Name = dto.Name,
                    Description = dto.Description,
                    PointsPerCurrency = dto.PointsPerCurrency,
                    CurrencyValue = dto.CurrencyValue,
                    MinOrderValue = dto.MinOrderValue,
                    PointsValidityDays = dto.PointsValidityDays,
                    IsActive = dto.IsActive
                };
                await _loyaltyProgramRepository.AddAsync(program);
            }
            else
            {
                program.Name = dto.Name;
                program.Description = dto.Description;
                program.PointsPerCurrency = dto.PointsPerCurrency;
                program.CurrencyValue = dto.CurrencyValue;
                program.MinOrderValue = dto.MinOrderValue;
                program.PointsValidityDays = dto.PointsValidityDays;
                program.IsActive = dto.IsActive;
                program.UpdatedAt = DateTime.UtcNow;
                await _loyaltyProgramRepository.UpdateAsync(program);
            }

            return StaticResponseBuilder<LoyaltyProgramDto>.BuildOk(new LoyaltyProgramDto
            {
                Id = program.Id,
                Name = program.Name,
                Description = program.Description,
                PointsPerCurrency = program.PointsPerCurrency,
                CurrencyValue = program.CurrencyValue,
                MinOrderValue = program.MinOrderValue,
                PointsValidityDays = program.PointsValidityDays,
                IsActive = program.IsActive
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar programa de fidelidade");
            return StaticResponseBuilder<LoyaltyProgramDto>.BuildError("Erro ao salvar programa");
        }
    }

    public async Task<ResponseDTO<CustomerLoyaltySummaryDto>> GetCustomerBalanceAsync(Guid tenantId, string customerPhone)
    {
        try
        {
            // Busca cliente pelo telefone e tenant
            var customer = await _customerRepository.GetByPhoneAsync(tenantId, customerPhone);
            if (customer == null)
                return StaticResponseBuilder<CustomerLoyaltySummaryDto>.BuildNotFound(null!);

            var balance = await _customerLoyaltyRepository.GetByCustomerAndTenantAsync(customer.Id, tenantId);
            var program = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);

            return StaticResponseBuilder<CustomerLoyaltySummaryDto>.BuildOk(new CustomerLoyaltySummaryDto
            {
                Balance = balance?.Balance ?? 0,
                TotalEarned = balance?.TotalEarned ?? 0,
                Program = program != null ? new LoyaltyProgramDto
                {
                    Id = program.Id,
                    Name = program.Name,
                    Description = program.Description,
                    PointsPerCurrency = program.PointsPerCurrency,
                    CurrencyValue = program.CurrencyValue,
                    MinOrderValue = program.MinOrderValue,
                    PointsValidityDays = program.PointsValidityDays,
                    IsActive = program.IsActive
                } : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter saldo de fidelidade");
            return StaticResponseBuilder<CustomerLoyaltySummaryDto>.BuildError("Erro ao obter saldo");
        }
    }

    public async Task ProcessOrderPointsAsync(Guid orderId, Guid tenantId)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId, tenantId);
            if (order == null) return;
            var program = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);

            // Regras básicas: Programa existe, está ativo, e pedido atinge valor mínimo
            if (program == null || !program.IsActive || order.Total < program.MinOrderValue) return;

            // Calcula pontos
            int pointsToEarn = (int)Math.Floor(order.Total * program.PointsPerCurrency);
            if (pointsToEarn <= 0) return;

            // Busca saldo
            var balance = await _customerLoyaltyRepository.GetByCustomerAndTenantAsync(order.CustomerId, tenantId);

            if (balance == null)
            {
                balance = new CustomerLoyaltyBalanceEntity
                {
                    CustomerId = order.CustomerId,
                    TenantId = tenantId,
                    Balance = 0,
                    TotalEarned = 0
                };
                await _customerLoyaltyRepository.AddAsync(balance);
                // Precisa salvar para gerar ID se for Identity, mas EF Core gerencia tracking.
                // Como CustomerLoyaltyRepository usa UnitOfWork implícito via contexto compartilhado, ok.
            }

            // Cria transação
            var transaction = new LoyaltyTransactionEntity
            {
                CustomerLoyaltyBalanceId = balance.Id, // EF Core vai resolver isso se balance for tracked
                CustomerLoyaltyBalance = balance, // Vínculo direto
                OrderId = order.Id,
                Points = pointsToEarn,
                Type = ELoyaltyTransactionType.Earn,
                Description = $"Pontos do pedido #{order.Id}",
                ExpiresAt = program.PointsValidityDays.HasValue 
                    ? DateTime.UtcNow.AddDays(program.PointsValidityDays.Value) 
                    : null
            };

            await _customerLoyaltyRepository.AddTransactionAsync(transaction);

            // Atualiza saldo
            balance.Balance += pointsToEarn;
            balance.TotalEarned += pointsToEarn;
            balance.LastActivityAt = DateTime.UtcNow;

            await _customerLoyaltyRepository.UpdateAsync(balance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar pontos para o pedido {OrderId}", orderId);
            // Não relança exceção para não bloquear o fluxo do pedido
        }
    }

    public async Task<ResponseDTO<LoyaltyProgramDto>> ToggleStatus(Guid tenantId, Guid id, bool status)
    {
        try
        {
            var program = await _loyaltyProgramRepository.FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == id);

            if (program == null)
                return StaticResponseBuilder<LoyaltyProgramDto>.BuildNotFound(null!);

            program.IsActive = status;
            program.UpdatedAt = DateTime.UtcNow;
            await _loyaltyProgramRepository.UpdateAsync(program);
            return StaticResponseBuilder<LoyaltyProgramDto>.BuildOk(new LoyaltyProgramDto
            {
                Id = program.Id,
                Name = program.Name,
                Description = program.Description,
                PointsPerCurrency = program.PointsPerCurrency,
                CurrencyValue = program.CurrencyValue,
                MinOrderValue = program.MinOrderValue,
                PointsValidityDays = program.PointsValidityDays,
                IsActive = program.IsActive
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do programa de fidelidade");
            return StaticResponseBuilder<LoyaltyProgramDto>.BuildError("Erro ao alterar status do programa");
        }
    }
}
