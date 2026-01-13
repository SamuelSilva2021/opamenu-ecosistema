import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';
import '../providers/dashboard_provider.dart';
import '../widgets/stat_card.dart';
import '../widgets/recent_orders_table.dart';

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
                    const SizedBox(height: 32),
                    
                    // Stats Grid
                    LayoutBuilder(
                      builder: (context, constraints) {
                        // Responsividade básica
                        final width = constraints.maxWidth;
                        final crossAxisCount = width > 1100 ? 4 : (width > 800 ? 2 : 1);
                        final aspectRatio = width > 1100 ? 1.4 : 1.6;
                        
                        return GridView.count(
                          crossAxisCount: crossAxisCount,
                          crossAxisSpacing: 24,
                          mainAxisSpacing: 24,
                          shrinkWrap: true,
                          physics: const NeverScrollableScrollPhysics(),
                          childAspectRatio: aspectRatio,
                          children: [
                            StatCard(
                              title: 'Receita Total',
                              value: 'R\$ ${summary.totalRevenue.toStringAsFixed(2)}',
                              growth: summary.totalRevenueGrowth,
                              icon: Icons.attach_money,
                              color: Colors.green,
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
}
