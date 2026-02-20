using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Loyalty;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

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
            var programs = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);
            var program = programs.FirstOrDefault();
            if (program == null)
                return StaticResponseBuilder<LoyaltyProgramDto>.BuildOk(null!);

            return StaticResponseBuilder<LoyaltyProgramDto>.BuildOk(MapToDto(program));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter programa de fidelidade");
            return StaticResponseBuilder<LoyaltyProgramDto>.BuildError("Erro ao obter programa");
        }
    }

    public async Task<ResponseDTO<IEnumerable<LoyaltyProgramDto>>> GetAllProgramsAsync(Guid tenantId)
    {
        try
        {
            var programs = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);
            return StaticResponseBuilder<IEnumerable<LoyaltyProgramDto>>.BuildOk(programs.Select(MapToDto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter programas de fidelidade");
            return StaticResponseBuilder<IEnumerable<LoyaltyProgramDto>>.BuildError("Erro ao obter lista de programas");
        }
    }

    public async Task<ResponseDTO<LoyaltyProgramDto>> CreateProgramAsync(Guid tenantId, CreateLoyaltyProgramDto dto)
    {
        try
        {
            var program = new LoyaltyProgramEntity
            {
                TenantId = tenantId,
                Name = dto.Name,
                Description = dto.Description,
                PointsPerCurrency = dto.PointsPerCurrency,
                CurrencyValue = dto.CurrencyValue,
                MinOrderValue = dto.MinOrderValue,
                PointsValidityDays = dto.PointsValidityDays,
                IsActive = dto.IsActive,
                Type = dto.Type,
                TargetCount = dto.TargetCount,
                RewardType = dto.RewardType,
                RewardValue = dto.RewardValue,
                Filters = dto.Filters.Select(f => new LoyaltyProgramFilterEntity
                {
                    ProductId = f.ProductId,
                    CategoryId = f.CategoryId
                }).ToList()
            };
            await _loyaltyProgramRepository.AddAsync(program);

            return StaticResponseBuilder<LoyaltyProgramDto>.BuildOk(MapToDto(program));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar programa de fidelidade");
            return StaticResponseBuilder<LoyaltyProgramDto>.BuildError("Erro ao criar programa");
        }
    }

    public async Task<ResponseDTO<LoyaltyProgramDto>> UpdateProgramAsync(Guid tenantId, Guid programId, CreateLoyaltyProgramDto dto)
    {
        try
        {
            var programs = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);
            var program = programs.FirstOrDefault(p => p.Id == programId);

            if (program == null)
                return StaticResponseBuilder<LoyaltyProgramDto>.BuildNotFound(null!);

            program.Name = dto.Name;
            program.Description = dto.Description;
            program.PointsPerCurrency = dto.PointsPerCurrency;
            program.CurrencyValue = dto.CurrencyValue;
            program.MinOrderValue = dto.MinOrderValue;
            program.PointsValidityDays = dto.PointsValidityDays;
            program.IsActive = dto.IsActive;
            program.Type = dto.Type;
            program.TargetCount = dto.TargetCount;
            program.RewardType = dto.RewardType;
            program.RewardValue = dto.RewardValue;
            program.UpdatedAt = DateTime.UtcNow;

            // Atualizar Filtros
            program.Filters.Clear();
            foreach (var filterDto in dto.Filters)
            {
                program.Filters.Add(new LoyaltyProgramFilterEntity
                {
                    LoyaltyProgramId = program.Id,
                    ProductId = filterDto.ProductId,
                    CategoryId = filterDto.CategoryId
                });
            }

            await _loyaltyProgramRepository.UpdateAsync(program);

            return StaticResponseBuilder<LoyaltyProgramDto>.BuildOk(MapToDto(program));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar programa de fidelidade");
            return StaticResponseBuilder<LoyaltyProgramDto>.BuildError("Erro ao atualizar programa");
        }
    }

    public async Task<ResponseDTO<CustomerLoyaltySummaryDto>> GetCustomerBalanceAsync(Guid tenantId, string customerPhone)
    {
        try
        {
            var customer = await _customerRepository.GetByPhoneAsync(tenantId, customerPhone);
            if (customer == null)
                return StaticResponseBuilder<CustomerLoyaltySummaryDto>.BuildNotFound(null!);

            // Multi-Wallet: Get All Balances
            var balances = await _customerLoyaltyRepository.GetAllBalancesAsync(customer.Id, tenantId);
            var programs = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);
            var activePrograms = programs.Where(p => p.IsActive).ToList();

            // Lazy Migration: If there is a legacy balance (ProgramId == null), assign it to the first active PointsPerValue program
            var legacyBalance = balances.FirstOrDefault(b => b.LoyaltyProgramId == null);
            if (legacyBalance != null && activePrograms.Any())
            {
                var targetProgram = activePrograms.FirstOrDefault(p => p.Type == ELoyaltyProgramType.PointsPerValue);
                if (targetProgram != null)
                {
                    _logger.LogInformation("Migrating legacy balance {BalanceId} for customer {CustomerId} to program {ProgramId}", 
                        legacyBalance.Id, customer.Id, targetProgram.Id);
                    
                    legacyBalance.LoyaltyProgramId = targetProgram.Id;
                    await _customerLoyaltyRepository.UpdateAsync(legacyBalance);
                    
                    balances = await _customerLoyaltyRepository.GetAllBalancesAsync(customer.Id, tenantId);
                }
            }
            
            var balancesDto = balances.Select(b => new CustomerLoyaltyBalanceDto
            {
                ProgramId = b.LoyaltyProgramId,
                ProgramName = b.LoyaltyProgram?.Name ?? "Saldo Geral",
                Balance = b.Balance
            }).ToList();

            var totalBalance = balances.Sum(b => b.Balance);
            var totalEarned = balances.Sum(b => b.TotalEarned);

            var firstProgram = activePrograms.FirstOrDefault();

            return StaticResponseBuilder<CustomerLoyaltySummaryDto>.BuildOk(new CustomerLoyaltySummaryDto
            {
                Balance = totalBalance,
                TotalEarned = totalEarned,
                Program = firstProgram != null ? MapToDto(firstProgram) : null,
                Balances = balancesDto
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
            var order = await _orderRepository.GetByIdForLoyaltyAsync(orderId, tenantId);
            if (order == null) return;
            var programs = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);

            var activePrograms = programs.Where(p => p.IsActive).ToList();
            if (!activePrograms.Any()) return;

            foreach (var program in activePrograms)
            {
                // Multi-Program: Check if points already awarded FOR TEHIS PROGRAM
                if (await _customerLoyaltyRepository.TransactionExistsAsync(order.Id, ELoyaltyTransactionType.Earn, program.Id))
                {
                    _logger.LogInformation("Pontos do pedido {OrderId} já processados para o programa {ProgramName}.", order.Id, program.Name);
                    continue;
                }

                if (order.Total < program.MinOrderValue) continue;

                int pointsToEarn = 0;
                string description = string.Empty;

                if (program.Type == ELoyaltyProgramType.PointsPerValue)
                {
                    decimal eligibleValue = order.Total;
                    _logger.LogInformation("Processando PointsPerValue. Total: {Total}, MinOrder: {Min}, Items: {ItemCount}", 
                        order.Total, program.MinOrderValue, order.Items.Count);

                    if (program.Filters != null && program.Filters.Any())
                    {
                        var excludedItemsValue = order.Items
                            .Where(orderItem => program.Filters.Any(f =>
                                (f.ProductId.HasValue && f.ProductId == orderItem.ProductId) ||
                                (f.CategoryId.HasValue && orderItem.Product != null && orderItem.Product.CategoryId == f.CategoryId)
                            ))
                            .Sum(i => i.Subtotal);

                        eligibleValue -= excludedItemsValue;
                        _logger.LogInformation("Valor Excluído: {Excluded}, Valor Elegível: {Eligible}", excludedItemsValue, eligibleValue);
                    }

                    if (eligibleValue <= 0) 
                    {
                        _logger.LogInformation("Valor elegível é 0 ou menor. Pulando.");
                        continue;
                    }

                    if (program.CurrencyValue > 0)
                    {
                        pointsToEarn = (int)Math.Floor((eligibleValue / program.CurrencyValue) * program.PointsPerCurrency);
                    }
                    else
                    {
                        pointsToEarn = (int)Math.Floor(eligibleValue * program.PointsPerCurrency);
                    }
                    
                    _logger.LogInformation("Pontos Calculados: {Points} (CurrencyVal: {CV}, PointsPerCurr: {PPC})", 
                        pointsToEarn, program.CurrencyValue, program.PointsPerCurrency);

                    description = $"Pontos do pedido #{order.Id} ({program.Name})";
                }
                else if (program.Type == ELoyaltyProgramType.OrderCount)
                {
                    pointsToEarn = 1;
                    description = $"Contagem de pedido #{order.Id} ({program.Name})";
                }
                else if (program.Type == ELoyaltyProgramType.ItemCount)
                {
                    _logger.LogInformation("Processando ItemCount Program: {ProgramName} ({ProgramId}). Order Items: {ItemCount}", program.Name, program.Id, order.Items.Count);
                    
                    if (program.Filters != null)
                    {
                        foreach(var f in program.Filters)
                        {
                            _logger.LogInformation("Filtro do Programa -> Cat: {CatId}, Prod: {ProdId}", f.CategoryId, f.ProductId);
                        }
                    }

                    foreach(var item in order.Items)
                    {
                         _logger.LogInformation("Item do Pedido -> Name: {Name}, ProdId: {ProdId}, CatId: {CatId}", 
                            item.ProductName, item.ProductId, item.Product?.CategoryId);
                    }

                    List<OrderItemEntity> eligibleItems;
                    if (program.Filters == null || !program.Filters.Any())
                    {
                        eligibleItems = order.Items.ToList();
                        _logger.LogInformation("Contador de Itens: Sem filtros definidos. Todos os {Count} itens são elegíveis.", eligibleItems.Count);
                    }
                    else
                    {
                        eligibleItems = order.Items.Where(item => 
                            program.Filters.Any(f => 
                                (f.ProductId.HasValue && f.ProductId == item.ProductId) || 
                                (f.CategoryId.HasValue && item.Product != null && item.Product.CategoryId == f.CategoryId)
                            )
                        ).ToList();
                    }

                    pointsToEarn = eligibleItems.Sum(i => i.Quantity);
                    _logger.LogInformation("Contador de Itens: Encontrados {Count} itens elegíveis. Total de Pontos: {Points}", eligibleItems.Count, pointsToEarn);
                    
                    description = $"Contagem de itens do pedido #{order.Id} ({program.Name})";
                }

                if (pointsToEarn <= 0) continue;

                // Multi-Wallet: Get or Create Balance Specifc for THIS Program
                var balance = await _customerLoyaltyRepository.GetByCustomerAndProgramAsync(order.CustomerId, program.Id);
                
                if (balance == null)
                {
                    balance = new CustomerLoyaltyBalanceEntity
                    {
                        CustomerId = order.CustomerId,
                        TenantId = tenantId,
                        LoyaltyProgramId = program.Id, // Link to Program
                        Balance = 0,
                        TotalEarned = 0
                    };
                    await _customerLoyaltyRepository.AddAsync(balance);
                }

                var transaction = new LoyaltyTransactionEntity
                {
                    CustomerLoyaltyBalanceId = balance.Id,
                    CustomerLoyaltyBalance = balance,
                    OrderId = order.Id,
                    Points = pointsToEarn,
                    Type = ELoyaltyTransactionType.Earn,
                    Description = description,
                    ExpiresAt = program.PointsValidityDays.HasValue
                        ? DateTime.UtcNow.AddDays(program.PointsValidityDays.Value)
                        : null
                };

                await _customerLoyaltyRepository.AddTransactionAsync(transaction);
                
                balance.Balance += pointsToEarn;
                balance.TotalEarned += pointsToEarn;
                balance.LastActivityAt = DateTime.UtcNow;
                await _customerLoyaltyRepository.UpdateAsync(balance);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar fidelidade para o pedido {OrderId}", orderId);
        }
    }

    public async Task<ResponseDTO<LoyaltyProgramDto>> ToggleStatus(Guid tenantId, Guid id, bool status)
    {
        try
        {
            var programs = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);
            var program = programs.FirstOrDefault(p => p.Id == id);

            if (program == null)
                return StaticResponseBuilder<LoyaltyProgramDto>.BuildNotFound(null!);

            program.IsActive = status;
            program.UpdatedAt = DateTime.UtcNow;
            await _loyaltyProgramRepository.UpdateAsync(program);
            return StaticResponseBuilder<LoyaltyProgramDto>.BuildOk(MapToDto(program));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do programa de fidelidade");
            return StaticResponseBuilder<LoyaltyProgramDto>.BuildError("Erro ao alterar status do programa");
        }
    }

    public async Task<ResponseDTO<bool>> DeleteProgramAsync(Guid tenantId, Guid id)
    {
        try
        {
            var programs = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);
            var program = programs.FirstOrDefault(p => p.Id == id);

            if (program == null)
                return StaticResponseBuilder<bool>.BuildNotFound(false);

            await _loyaltyProgramRepository.DeleteAsync(program);
            return StaticResponseBuilder<bool>.BuildOk(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir programa de fidelidade");
            return StaticResponseBuilder<bool>.BuildError("Erro ao excluir programa");
        }
    }

    private LoyaltyProgramDto MapToDto(LoyaltyProgramEntity program)
    {
        return new LoyaltyProgramDto
        {
            Id = program.Id,
            Name = program.Name,
            Description = program.Description,
            PointsPerCurrency = program.PointsPerCurrency,
            CurrencyValue = program.CurrencyValue,
            MinOrderValue = program.MinOrderValue,
            PointsValidityDays = program.PointsValidityDays,
            IsActive = program.IsActive,
            Type = program.Type,
            TargetCount = program.TargetCount,
            RewardType = program.RewardType,
            RewardValue = program.RewardValue,
            Filters = program.Filters?.Select(f => new LoyaltyProgramFilterDto
            {
                ProductId = f.ProductId,
                CategoryId = f.CategoryId
            }).ToList() ?? new List<LoyaltyProgramFilterDto>()
        };
    }
}
