import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../tables/domain/entities/table_entity.dart';
import '../../domain/entities/tab_entity.dart';
import '../pages/tab_details_page.dart';
import '../providers/tabs_controller_provider.dart';

class TabsList extends ConsumerWidget {
  final TableEntity table;
  final List<TabEntity> tabs;

  const TabsList({
    super.key,
    required this.table,
    required this.tabs,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    if (tabs.isEmpty) {
      return const Center(child: Text('Nenhuma comanda nesta mesa'));
    }

    final openTabs = tabs.where((t) => t.isOpen).toList(growable: false);
    final closedTabs = tabs.where((t) => !t.isOpen).toList(growable: false);
    final display = [...openTabs, ...closedTabs];

    return ListView.separated(
      padding: const EdgeInsets.all(16),
      itemCount: display.length,
      separatorBuilder: (_, _) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final tab = display[index];
        return Card(
          child: ListTile(
            title: Text(tab.name?.trim().isNotEmpty == true ? tab.name! : 'Comanda'),
            subtitle: Text(tab.isOpen ? 'Aberta' : 'Fechada'),
            trailing: const Icon(Icons.chevron_right),
            onTap: tab.isOpen
                ? () async {
                    final changed = await Navigator.of(context).push<bool>(
                      MaterialPageRoute(
                        builder: (_) => TabDetailsPage(table: table, tab: tab),
                      ),
                    );
                    if (changed == true) {
                      await ref
                          .read(TabsControllerProvider.provider.notifier)
                          .load(tableId: table.id, force: true);
                    }
                  }
                : null,
          ),
        );
      },
    );
  }
}
