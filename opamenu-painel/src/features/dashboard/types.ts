export interface RecentOrder {
  id: number;
  customerName: string;
  amount: number;
  createdAt: string;
}

export interface DashboardSummary {
  totalRevenue: number;
  totalRevenueGrowth: number;
  ordersToday: number;
  ordersTodayGrowth: number;
  totalOrders: number;
  totalOrdersGrowth: number;
  activeCustomers: number;
  activeCustomersGrowth: number;
  recentOrders: RecentOrder[];
}
