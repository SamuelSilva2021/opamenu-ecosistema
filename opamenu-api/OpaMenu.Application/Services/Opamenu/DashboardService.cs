using OpaMenu.Application.DTOs.Dashboard;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Infrastructure.Shared.Enums.Opamenu;
using OpaMenu.Application.Services.Interfaces.Opamenu;

namespace OpaMenu.Application.Services.Opamenu;

public class DashboardService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    ICurrentUserService currentUserService) : IDashboardService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<ResponseDTO<DashboardSummaryDto>> GetSummaryAsync()
    {
        try
        {
            var tenantId = _currentUserService.GetTenantGuid();
            if (tenantId == null)
                return StaticResponseBuilder<DashboardSummaryDto>.BuildError("Tenant não identificado.");

            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
            
            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfMonth.AddTicks(-1);
            
            var startOfToday = DateTime.SpecifyKind(now.Date, DateTimeKind.Utc);
            var endOfToday = startOfToday.AddDays(1).AddTicks(-1);
            
            var startOfYesterday = startOfToday.AddDays(-1);
            var endOfYesterday = startOfToday.AddTicks(-1);

            // Fetch current month orders
            var currentMonthOrders = (await _orderRepository.FindAsync(o => 
                o.TenantId == tenantId && 
                o.CreatedAt >= startOfMonth && 
                o.CreatedAt <= endOfMonth &&
                o.Status != EOrderStatus.Cancelled && 
                o.Status != EOrderStatus.Rejected)).ToList();
                
            // Fetch last month orders
            var lastMonthOrders = (await _orderRepository.FindAsync(o => 
                o.TenantId == tenantId && 
                o.CreatedAt >= startOfLastMonth && 
                o.CreatedAt <= endOfLastMonth &&
                o.Status != EOrderStatus.Cancelled && 
                o.Status != EOrderStatus.Rejected)).ToList();
                
            // Fetch today orders
            var todayOrders = currentMonthOrders
                .Where(o => o.CreatedAt >= startOfToday && o.CreatedAt <= endOfToday)
                .ToList();
            
            // Fetch yesterday orders
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
            var totalRevenue = currentMonthOrders.Sum(o => o.Total);
            var lastMonthRevenue = lastMonthOrders.Sum(o => o.Total);
            var revenueGrowth = CalculateGrowth(totalRevenue, lastMonthRevenue);
            
            var totalOrdersCount = currentMonthOrders.Count;
            var lastMonthOrdersCount = lastMonthOrders.Count;
            var ordersGrowth = CalculateGrowth(totalOrdersCount, lastMonthOrdersCount);
            
            var ordersTodayCount = todayOrders.Count;
            var ordersYesterdayCount = yesterdayOrders.Count;
            var ordersTodayGrowth = CalculateGrowth(ordersTodayCount, ordersYesterdayCount);

            var activeCustomersGrowth = 0.0;

            var summary = new DashboardSummaryDto
            {
                TotalRevenue = totalRevenue,
                TotalRevenueGrowth = revenueGrowth,
                OrdersToday = ordersTodayCount,
                OrdersTodayGrowth = ordersTodayGrowth,
                TotalOrders = totalOrdersCount,
                TotalOrdersGrowth = ordersGrowth,
                ActiveCustomers = activeCustomersCount,
                ActiveCustomersGrowth = activeCustomersGrowth,
                RecentOrders = recentOrdersDto
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

