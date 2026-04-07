import 'package:flutter/material.dart';

import '../../domain/entities/table_entity.dart';
import '../../../tabs/presentation/pages/tabs_page.dart';
import 'tables_empty_state.dart';

class TablesList extends StatelessWidget {
  final List<TableEntity> tables;

  const TablesList({
    super.key,
    required this.tables,
  });

  @override
  Widget build(BuildContext context) {
    if (tables.isEmpty) {
      return const TablesEmptyState();
    }

    final active = tables.where((t) => t.isActive).toList(growable: false);
    final inactive = tables.where((t) => !t.isActive).toList(growable: false);
    final display = [...active, ...inactive];

    return ListView.separated(
      padding: const EdgeInsets.all(16),
      itemCount: display.length,
      separatorBuilder: (_, _) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final table = display[index];
        return Card(
          child: ListTile(
            title: Text(table.name),
            subtitle: Text(table.isActive ? 'Disponível' : 'Inativa'),
            trailing: const Icon(Icons.chevron_right),
            onTap: table.isActive
                ? () {
                    Navigator.of(context).push(
                      MaterialPageRoute(
                        builder: (_) => TabsPage(table: table),
                      ),
                    );
                  }
                : null,
          ),
        );
      },
    );
  }
}
