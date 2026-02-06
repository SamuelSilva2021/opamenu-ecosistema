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

public class DailySaleDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

public class CategorySaleDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Quantity { get; set; }
}

public class RecentOrderDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
