import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/app_colors.dart';
import '../../../../core/domain/failures/failure.dart';
import '../providers/tabs_controller_provider.dart';
import '../widgets/tabs_error_state.dart';
import '../widgets/tabs_list.dart';
import 'tabs_page.dart';

class TabsPageState extends ConsumerState<TabsPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      ref.read(TabsControllerProvider.provider.notifier).load(
            tableId: widget.table.id,
            force: true,
          );
    });
  }

  @override
  Widget build(BuildContext context) {
    final tabsAsync = ref.watch(TabsControllerProvider.provider);

    return Scaffold(
      appBar: AppBar(
        title: Text(widget.table.name),
        actions: [
          IconButton(
            onPressed: () => ref
                .read(TabsControllerProvider.provider.notifier)
                .load(tableId: widget.table.id, force: true),
            icon: const Icon(Icons.refresh),
          ),
        ],
      ),
      body: Container(
        color: AppColors.surface,
        child: tabsAsync.when(
          data: (tabs) => TabsList(table: widget.table, tabs: tabs),
          error: (error, _) => TabsErrorState(
            message: error is Failure ? error.message : error.toString(),
          ),
          loading: () => const Center(child: CircularProgressIndicator()),
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () async {
          final name = await _askTabName(context);
          if (!context.mounted) return;
          await ref
              .read(TabsControllerProvider.provider.notifier)
              .openTab(tableId: widget.table.id, name: name);
        },
        icon: const Icon(Icons.add),
        label: const Text('Abrir comanda'),
      ),
    );
  }

  Future<String?> _askTabName(BuildContext context) async {
    var name = '';
    final result = await showDialog<String?>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Nome da comanda'),
        content: TextField(
          decoration: const InputDecoration(
            hintText: 'Opcional (ex: João, Família)',
          ),
          onChanged: (value) => name = value,
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(null),
            child: const Text('Cancelar'),
          ),
          FilledButton(
            onPressed: () => Navigator.of(context).pop(name),
            child: const Text('Abrir'),
          ),
        ],
      ),
    );
    return result;
  }
}

