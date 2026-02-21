using OpaMenu.Application.DTOs.Dashboard;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

public class DashboardService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    ICurrentUserService currentUserService) : IDashboardService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<ResponseDTO<DashboardSummaryDto>> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                return StaticResponseBuilder<DashboardSummaryDto>.BuildError("Tenant não identificado.");

            var now = DateTime.UtcNow;
            
            // Set default period to current month if not provided
            var start = startDate ?? new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = endDate ?? start.AddMonths(1).AddTicks(-1);
            
            // Ensure kinds are Utc
            start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

            // Calculate duration of selected period to find previous period
            var duration = end - start;
            var startOfPreviousPeriod = start.Subtract(duration).AddTicks(-1);
            var endOfPreviousPeriod = start.AddTicks(-1);

            // Fetch current period orders
            var currentPeriodOrders = (await _orderRepository.FindAsync(o => 
                o.TenantId == tenantId && 
                o.CreatedAt >= start && 
                o.CreatedAt <= end &&
                o.Status != EOrderStatus.Cancelled && 
                o.Status != EOrderStatus.Rejected)).ToList();
                
            // Fetch previous period orders
            var previousPeriodOrders = (await _orderRepository.FindAsync(o => 
                o.TenantId == tenantId && 
                o.CreatedAt >= startOfPreviousPeriod && 
                o.CreatedAt <= endOfPreviousPeriod &&
                o.Status != EOrderStatus.Cancelled && 
                o.Status != EOrderStatus.Rejected)).ToList();
                
            // Fetch today orders for the small metric (still relevant)
            var startOfToday = DateTime.SpecifyKind(now.Date, DateTimeKind.Utc);
            var endOfToday = startOfToday.AddDays(1).AddTicks(-1);
            var startOfYesterday = startOfToday.AddDays(-1);
            var endOfYesterday = startOfToday.AddTicks(-1);

            var todayOrders = (await _orderRepository.FindAsync(o => 
                o.TenantId == tenantId && 
                o.CreatedAt >= startOfToday && 
                o.CreatedAt <= endOfToday &&
                o.Status != EOrderStatus.Cancelled && 
                o.Status != EOrderStatus.Rejected)).ToList();
            
            var yesterdayOrders = (await _orderRepository.FindAsync(o => 
                o.TenantId == tenantId && 
                o.CreatedAt >= startOfYesterday && 
                o.CreatedAt <= endOfYesterday &&
                o.Status != EOrderStatus.Cancelled && 
                o.Status != EOrderStatus.Rejected)).ToList();
                
            // Active Customers (Total)
            var allCustomers = await _customerRepository.GetByTenantIdAsync(tenantId.Value);
            var activeCustomersCount = allCustomers.Count(); 
            
            // Recent Orders
            var recentOrders = await _orderRepository.FindOrderedAsync(
                o => o.TenantId == tenantId,
                o => o.CreatedAt,
                false
            );
            
            var recentOrdersDto = recentOrders.Take(5).Select(o => new RecentOrderDto 
            { 
                Id = o.Id,
                CustomerName = o.CustomerName,
                Amount = o.Total,
                CreatedAt = o.CreatedAt
            }).ToList();

            // Calculate metrics
            var totalRevenue = currentPeriodOrders.Sum(o => o.Total);
            var previousRevenue = previousPeriodOrders.Sum(o => o.Total);
            var revenueGrowth = CalculateGrowth(totalRevenue, previousRevenue);
            
            var totalOrdersCount = currentPeriodOrders.Count;
            var previousOrdersCount = previousPeriodOrders.Count;
            var ordersGrowth = CalculateGrowth(totalOrdersCount, previousOrdersCount);
            
            var ordersTodayCount = todayOrders.Count;
            var ordersYesterdayCount = yesterdayOrders.Count;
            var ordersTodayGrowth = CalculateGrowth(ordersTodayCount, ordersYesterdayCount);

            var activeCustomersGrowth = 0.0;

            // Average Ticket
            var averageTicket = totalOrdersCount > 0 ? totalRevenue / totalOrdersCount : 0;

            // Daily Sales (Last 7 Days or selected period if shorter?) 
            // Let's keep last 7 days for the chart for now, or use the selected period if it's within a reasonable range
            var chartStartDate = start;
            var chartEndDate = end;
            
            // If period is longer than 30 days, maybe group by week? 
            // For now, let's just use the selected period for the chart if it's <= 31 days
            var chartDuration = (end - start).TotalDays;
            
            var dailySales = new List<DailySaleDto>();
            if (chartDuration <= 31)
            {
                var days = (int)Math.Ceiling(chartDuration);
                dailySales = Enumerable.Range(0, days + 1)
                    .Select(offset => start.AddDays(offset))
                    .Where(date => date <= end)
                    .Select(date => new DailySaleDto
                    {
                        Date = date.ToString("dd/MM"),
                        Total = currentPeriodOrders
                            .Where(o => o.CreatedAt.Date == date.Date)
                            .Sum(o => o.Total)
                    }).ToList();
            }
            else
            {
                // Fallback to last 7 days if period is too long for daily chart
                var weekAgo = startOfToday.AddDays(-6);
                var lastWeekOrders = (await _orderRepository.FindAsync(o => 
                    o.TenantId == tenantId && 
                    o.CreatedAt >= weekAgo && 
                    o.Status != EOrderStatus.Cancelled && 
                    o.Status != EOrderStatus.Rejected)).ToList();

                dailySales = Enumerable.Range(0, 7)
                    .Select(offset => weekAgo.AddDays(offset))
                    .Select(date => new DailySaleDto
                    {
                        Date = date.ToString("dd/MM"),
                        Total = lastWeekOrders
                            .Where(o => o.CreatedAt.Date == date.Date)
                            .Sum(o => o.Total)
                    }).ToList();
            }

            // Category Sales (Distribution)
            var categorySales = currentPeriodOrders
                .SelectMany(o => o.Items ?? new List<OrderItemEntity>())
                .Where(i => i.Product != null && i.Product.Category != null)
                .GroupBy(i => i.Product!.Category!.Name)
                .Select(g => new CategorySaleDto
                {
                    CategoryName = g.Key,
                    Total = g.Sum(i => i.UnitPrice * i.Quantity),
                    Quantity = g.Sum(i => i.Quantity)
                })
                .OrderByDescending(c => c.Total)
                .Take(5)
                .ToList();

            var summary = new DashboardSummaryDto
            {
                TotalRevenue = totalRevenue,
                TotalRevenueGrowth = revenueGrowth,
                OrdersToday = ordersTodayCount,
                OrdersTodayGrowth = ordersTodayGrowth,
                TotalOrders = totalOrdersCount,
                TotalOrdersGrowth = (decimal)ordersGrowth,
                ActiveCustomers = activeCustomersCount,
                ActiveCustomersGrowth = activeCustomersGrowth,
                AverageTicket = averageTicket,
                RecentOrders = recentOrdersDto,
                DailySales = dailySales,
                CategorySales = categorySales
            };

            return StaticResponseBuilder<DashboardSummaryDto>.BuildOk(summary);
        }
        catch (Exception ex)
        {
            return StaticResponseBuilder<DashboardSummaryDto>.BuildErrorResponse(ex);
        }
    }

    private static double CalculateGrowth(decimal current, decimal previous)
    {
        if (previous == 0) return current > 0 ? 100 : 0;
        return (double)((current - previous) / previous * 100);
    }
    
    private static double CalculateGrowth(int current, int previous)
    {
        if (previous == 0) return current > 0 ? 100 : 0;
        return (double)(current - previous) / previous * 100;
    }
}

