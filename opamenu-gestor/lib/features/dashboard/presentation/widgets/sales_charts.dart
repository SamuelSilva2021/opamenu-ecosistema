import 'package:flutter/material.dart';
import 'package:fl_chart/fl_chart.dart';
import '../../domain/models/dashboard_summary_dto.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';

class SalesCharts extends StatelessWidget {
  final List<DailySaleDto> dailySales;
  final List<CategorySaleDto> categorySales;

  const SalesCharts({
    super.key,
    required this.dailySales,
    required this.categorySales,
  });

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Row(
          children: [
            Expanded(
              flex: 2,
              child: _buildWeeklySalesChart(),
            ),
            const SizedBox(width: 24),
            Expanded(
              flex: 1,
              child: _buildCategoryDistribution(),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildWeeklySalesChart() {
    return Container(
      height: 400,
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.05),
            blurRadius: 10,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Vendas nos Últimos 7 Dias',
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
              color: AppColors.textPrimary,
            ),
          ),
          const SizedBox(height: 32),
          Expanded(
            child: BarChart(
              BarChartData(
                alignment: BarChartAlignment.spaceAround,
                maxY: _getMaxY(),
                barTouchData: BarTouchData(enabled: true),
                titlesData: FlTitlesData(
                  show: true,
                  bottomTitles: AxisTitles(
                    sideTitles: SideTitles(
                      showTitles: true,
                      getTitlesWidget: (value, meta) {
                        if (value.toInt() < 0 || value.toInt() >= dailySales.length) return const SizedBox();
                        return Text(
                          dailySales[value.toInt()].date,
                          style: const TextStyle(fontSize: 12),
                        );
                      },
                    ),
                  ),
                  leftTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
                  topTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
                  rightTitles: const AxisTitles(sideTitles: SideTitles(showTitles: false)),
                ),
                gridData: const FlGridData(show: false),
                borderData: FlBorderData(show: false),
                barGroups: dailySales.asMap().entries.map((e) {
                  return BarChartGroupData(
                    x: e.key,
                    barRods: [
                      BarChartRodData(
                        toY: e.value.total,
                        color: AppColors.primary,
                        width: 16,
                        borderRadius: const BorderRadius.vertical(top: Radius.circular(4)),
                      ),
                    ],
                  );
                }).toList(),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildCategoryDistribution() {
    return Container(
      height: 400,
      padding: const EdgeInsets.all(24),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.05),
            blurRadius: 10,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Vendas por Categoria',
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
              color: AppColors.textPrimary,
            ),
          ),
          const SizedBox(height: 32),
          Expanded(
            child: PieChart(
              PieChartData(
                sectionsSpace: 2,
                centerSpaceRadius: 40,
                sections: categorySales.asMap().entries.map((e) {
                  final colors = [Colors.orange, Colors.blue, Colors.green, Colors.purple, Colors.red];
                  return PieChartSectionData(
                    color: colors[e.key % colors.length],
                    value: e.value.total,
                    title: '${e.value.total.toInt()}',
                    radius: 50,
                    titleStyle: const TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                      color: Colors.white,
                    ),
                  );
                }).toList(),
              ),
            ),
          ),
          const SizedBox(height: 16),
          // Legend
          ...categorySales.asMap().entries.map((e) {
             final colors = [Colors.orange, Colors.blue, Colors.green, Colors.purple, Colors.red];
             return Padding(
               padding: const EdgeInsets.symmetric(vertical: 2),
               child: Row(
                 children: [
                   Container(width: 8, height: 8, color: colors[e.key % colors.length]),
                   const SizedBox(width: 8),
                   Expanded(
                     child: Text(
                       e.value.categoryName,
                       style: const TextStyle(fontSize: 12),
                       overflow: TextOverflow.ellipsis,
                     ),
                   ),
                 ],
               ),
             );
          }),
        ],
      ),
    );
  }

  double _getMaxY() {
    if (dailySales.isEmpty) return 100;
    final max = dailySales.map((e) => e.total).reduce((a, b) => a > b ? a : b);
    return max * 1.2;
  }
}
