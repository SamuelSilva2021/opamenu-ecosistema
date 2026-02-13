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

    public async Task<ResponseDTO<LoyaltyProgramDto>> UpsertProgramAsync(Guid tenantId, CreateLoyaltyProgramDto dto)
    {
        try
        {
            var programs = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);
            LoyaltyProgramEntity? program = null;
            
            // Se for uma atualização (contém ID ou buscamos pelo primeiro existente para compatibilidade v1)
            // No futuro o Painel passará o ID para edição de programas específicos.
            // Para Phase 4, se o Painel não enviar ID, criamos um NOVO.
            // Mas para manter compatibilidade com a migração, vamos buscar se o usuário enviou um ID.
            
            // Nota: CreateLoyaltyProgramDto ainda não tem Id, mas podemos usar um UpdateLoyaltyProgramDto se necessário.
            // Por enquanto, se houver um programa e o nome for igual, atualizamos. Senão, criamos.
            // Para simplificar a Fase 4, vamos sempre criar se for um Post novo, ou atualizar se houver lógica de ID.
            
            program = programs.FirstOrDefault(); // Lógica V1: atualiza o primeiro.

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
            }

            return StaticResponseBuilder<LoyaltyProgramDto>.BuildOk(MapToDto(program));
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
            var customer = await _customerRepository.GetByPhoneAsync(tenantId, customerPhone);
            if (customer == null)
                return StaticResponseBuilder<CustomerLoyaltySummaryDto>.BuildNotFound(null!);

            var balance = await _customerLoyaltyRepository.GetByCustomerAndTenantAsync(customer.Id, tenantId);
            var programs = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);
            var program = programs.FirstOrDefault();

            return StaticResponseBuilder<CustomerLoyaltySummaryDto>.BuildOk(new CustomerLoyaltySummaryDto
            {
                Balance = balance?.Balance ?? 0,
                TotalEarned = balance?.TotalEarned ?? 0,
                Program = program != null ? MapToDto(program) : null
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
            var programs = await _loyaltyProgramRepository.GetByTenantIdAsync(tenantId);

            var activePrograms = programs.Where(p => p.IsActive).ToList();
            if (!activePrograms.Any()) return;

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
            }

            int totalPointsInOrder = 0;

            foreach (var program in activePrograms)
            {
                if (order.Total < program.MinOrderValue) continue;

                int pointsToEarn = 0;
                string description = string.Empty;

                if (program.Type == ELoyaltyProgramType.PointsPerValue)
                {
                    pointsToEarn = (int)Math.Floor(order.Total * program.PointsPerCurrency);
                    description = $"Pontos do pedido #{order.Id} ({program.Name})";
                }
                else if (program.Type == ELoyaltyProgramType.OrderCount)
                {
                    pointsToEarn = 1;
                    description = $"Contagem de pedido #{order.Id} ({program.Name})";
                }
                else if (program.Type == ELoyaltyProgramType.ItemCount)
                {
                    var eligibleItems = order.Items.Where(item => 
                        program.Filters.Any(f => 
                            (f.ProductId.HasValue && f.ProductId == item.ProductId) || 
                            (f.CategoryId.HasValue && item.Product != null && item.Product.CategoryId == f.CategoryId)
                        )
                    );

                    pointsToEarn = eligibleItems.Sum(i => i.Quantity);
                    description = $"Contagem de itens do pedido #{order.Id} ({program.Name})";
                }

                if (pointsToEarn <= 0) continue;

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
                totalPointsInOrder += pointsToEarn;
            }

            if (totalPointsInOrder > 0)
            {
                balance.Balance += totalPointsInOrder;
                balance.TotalEarned += totalPointsInOrder;
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
