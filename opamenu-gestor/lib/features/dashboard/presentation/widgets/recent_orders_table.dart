import 'package:flutter/material.dart';
import '../../domain/models/recent_order_dto.dart';

class RecentOrdersTable extends StatelessWidget {
  final List<RecentOrderDto> orders;

  const RecentOrdersTable({super.key, required this.orders});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.05),
            blurRadius: 10,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Pedidos Recentes',
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 16),
          SingleChildScrollView(
            scrollDirection: Axis.horizontal,
            child: DataTable(
              columnSpacing: 32,
              horizontalMargin: 0,
              columns: const [
                DataColumn(label: Text('ID Pedido')),
                DataColumn(label: Text('Cliente')),
                DataColumn(label: Text('Data')),
                DataColumn(label: Text('Valor')),
                DataColumn(label: Text('Status')),
              ],
              rows: orders.map((order) {
                return DataRow(
                  cells: [
                    DataCell(Text('#${order.id}')),
                    DataCell(Text(order.customerName)),
                    DataCell(Text(_formatDate(order.createdAt))),
                    DataCell(Text('R\$ ${order.amount.toStringAsFixed(2)}')),
                    DataCell(
                      Container(
                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                        decoration: BoxDecoration(
                          color: Colors.green.withValues(alpha: 0.1),
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: const Text(
                          'Concluído',
                          style: TextStyle(
                            color: Colors.green,
                            fontWeight: FontWeight.bold,
                            fontSize: 12,
                          ),
                        ),
                      ),
                    ),
                  ],
                );
              }).toList(),
            ),
          ),
        ],
      ),
    );
  }

  String _formatDate(DateTime date) {
    return '${date.day.toString().padLeft(2, '0')}/${date.month.toString().padLeft(2, '0')}/${date.year} ${date.hour.toString().padLeft(2, '0')}:${date.minute.toString().padLeft(2, '0')}';
  }
}
