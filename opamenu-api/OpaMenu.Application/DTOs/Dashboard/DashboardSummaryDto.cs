namespace OpaMenu.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public double TotalRevenueGrowth { get; set; }
    
    public int OrdersToday { get; set; }
    public double OrdersTodayGrowth { get; set; }
    
    public int TotalOrders { get; set; }
    public double TotalOrdersGrowth { get; set; }
    
    public int ActiveCustomers { get; set; }
    public double ActiveCustomersGrowth { get; set; }
    
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
}

public class RecentOrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
