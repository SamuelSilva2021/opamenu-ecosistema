import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import '../../domain/models/recent_order_dto.dart';

class RecentOrdersTable extends StatelessWidget {
  final List<RecentOrderDto> orders;

  const RecentOrdersTable({super.key, required this.orders});

  @override
  Widget build(BuildContext context) {
    final currencyFormat = NumberFormat.currency(locale: 'pt_BR', symbol: 'R\$');
    final dateFormat = DateFormat('dd/MM/yyyy HH:mm', 'pt_BR');

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
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text(
                'Pedidos Recentes',
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.bold,
                ),
              ),
              TextButton(
                onPressed: () {}, // Link to orders page
                child: const Text('Ver todos'),
              ),
            ],
          ),
          const SizedBox(height: 16),
          if (orders.isEmpty)
            const Center(
              child: Padding(
                padding: EdgeInsets.symmetric(vertical: 32),
                child: Text('Nenhum pedido recente encontrado'),
              ),
            )
          else
            SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: ConstrainedBox(
                constraints: BoxConstraints(minWidth: MediaQuery.of(context).size.width - 350),
                child: DataTable(
                  columnSpacing: 24,
                  horizontalMargin: 0,
                  columns: const [
                    DataColumn(label: Text('REF')),
                    DataColumn(label: Text('CLIENTE')),
                    DataColumn(label: Text('DATA')),
                    DataColumn(label: Text('VALOR')),
                    DataColumn(label: Text('STATUS')),
                  ],
                  rows: orders.map((order) {
                    return DataRow(
                      cells: [
                        DataCell(
                          Text(
                            order.id.length > 8 ? order.id.substring(0, 8).toUpperCase() : order.id.toUpperCase(),
                            style: const TextStyle(fontWeight: FontWeight.w500, fontFamily: 'monospace'),
                          ),
                        ),
                        DataCell(Text(order.customerName)),
                        DataCell(Text(dateFormat.format(order.createdAt))),
                        DataCell(
                          Text(
                            currencyFormat.format(order.amount),
                            style: const TextStyle(fontWeight: FontWeight.bold),
                          ),
                        ),
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
            ),
        ],
      ),
    );
  }
}
