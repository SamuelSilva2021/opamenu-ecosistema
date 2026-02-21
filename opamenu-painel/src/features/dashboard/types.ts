export interface RecentOrder {
  id: string;
  customerName: string;
  amount: number;
  createdAt: string;
}

export interface DailySale {
  date: string;
  total: number;
}

export interface CategorySale {
  categoryName: string;
  total: number;
  quantity: number;
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
  averageTicket: number;
  recentOrders: RecentOrder[];
  dailySales: DailySale[];
  categorySales: CategorySale[];
}
