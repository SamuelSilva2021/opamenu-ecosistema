import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';
import '../providers/dashboard_provider.dart';
import '../widgets/stat_card.dart';
import '../widgets/recent_orders_table.dart';
import '../widgets/sales_charts.dart';
import '../../domain/models/dashboard_summary_dto.dart';

class DashboardPage extends ConsumerWidget {
  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final summaryAsync = ref.watch(dashboardSummaryProvider);

    return summaryAsync.when(
      data: (summary) => SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Dashboard',
                      style: TextStyle(
                        fontSize: 28,
                        fontWeight: FontWeight.bold,
                        color: AppColors.textPrimary,
                      ),
                    ),
                    const SizedBox(height: 8),
                    const Text(
                      'Visão geral do seu negócio hoje',
                      style: TextStyle(
                        color: AppColors.textSecondary,
                        fontSize: 16,
                      ),
                    ),
                  ],
                ),
                ElevatedButton.icon(
                  onPressed: () => ref.invalidate(dashboardSummaryProvider),
                  icon: const Icon(Icons.sync),
                  label: const Text('Sincronizar'),
                  style: ElevatedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 32),
            
            // Stats Grid
            LayoutBuilder(
              builder: (context, constraints) {
                final width = constraints.maxWidth;
                final crossAxisCount = width > 1200 ? 5 : (width > 900 ? 3 : (width > 600 ? 2 : 1));
                final aspectRatio = width > 1200 ? 1.6 : 1.8;
                
                return GridView.count(
                  crossAxisCount: crossAxisCount,
                  crossAxisSpacing: 16,
                  mainAxisSpacing: 16,
                  shrinkWrap: true,
                  physics: const NeverScrollableScrollPhysics(),
                  childAspectRatio: aspectRatio,
                  children: [
                    StatCard(
                      title: 'Receita Total',
                      value: NumberFormat.currency(locale: 'pt_BR', symbol: 'R\$').format(summary.totalRevenue),
                      growth: summary.totalRevenueGrowth,
                      icon: Icons.attach_money,
                      color: Colors.green,
                      isCurrency: true,
                    ),
                    StatCard(
                      title: 'Ticket Médio',
                      value: NumberFormat.currency(locale: 'pt_BR', symbol: 'R\$').format(summary.averageTicket),
                      growth: 0,
                      icon: Icons.analytics,
                      color: Colors.teal,
                      isCurrency: true,
                    ),
                    StatCard(
                      title: 'Pedidos Hoje',
                      value: summary.ordersToday.toString(),
                      growth: summary.ordersTodayGrowth,
                      icon: Icons.shopping_cart,
                      color: Colors.orange,
                    ),
                    StatCard(
                      title: 'Total Pedidos',
                      value: summary.totalOrders.toString(),
                      growth: summary.totalOrdersGrowth,
                      icon: Icons.assignment,
                      color: Colors.blue,
                    ),
                    StatCard(
                      title: 'Clientes Ativos',
                      value: summary.activeCustomers.toString(),
                      growth: summary.activeCustomersGrowth,
                      icon: Icons.people,
                      color: Colors.purple,
                    ),
                  ],
                );
              },
            ),
            
            const SizedBox(height: 32),

            // Charts
            LayoutBuilder(
              builder: (context, constraints) {
                if (constraints.maxWidth > 800) {
                  return SalesCharts(
                    dailySales: summary.dailySales, 
                    categorySales: summary.categorySales
                  );
                } else {
                  return _buildMobileCharts(summary);
                }
              },
            ),
            
            const SizedBox(height: 32),
            
            // Recent Orders
            RecentOrdersTable(orders: summary.recentOrders),
          ],
        ),
      ),
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (error, stack) => Center(
        child: Text('Erro ao carregar dashboard: $error'),
      ),
    );
  }

  Widget _buildMobileCharts(DashboardSummaryDto summary) {
    return Column(
      children: [
        SizedBox(
          height: 300, 
          child: SalesCharts(dailySales: summary.dailySales, categorySales: []),
        ),
        const SizedBox(height: 16),
        SizedBox(
          height: 300, 
          child: SalesCharts(dailySales: [], categorySales: summary.categorySales),
        ),
      ],
    );
  }
}
