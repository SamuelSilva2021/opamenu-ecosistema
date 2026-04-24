namespace OpaMenu.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public double TotalRevenueGrowth { get; set; }
    
    public int OrdersToday { get; set; }
    public double OrdersTodayGrowth { get; set; }
    
    public int TotalOrders { get; set; }
    public decimal TotalOrdersGrowth { get; set; }
    
    public decimal AverageTicket { get; set; }
    
    public int ActiveCustomers { get; set; }
    public double ActiveCustomersGrowth { get; set; }
    
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
    public List<DailySaleDto> DailySales { get; set; } = new();
    public List<CategorySaleDto> CategorySales { get; set; } = new();
}
