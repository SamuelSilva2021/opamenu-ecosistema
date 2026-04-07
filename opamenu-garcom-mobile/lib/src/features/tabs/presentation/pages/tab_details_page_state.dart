import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/app_colors.dart';
import '../../../../core/domain/failures/failure.dart';
import '../../../products/presentation/pages/catalog_page.dart';
import '../../../tables/domain/entities/table_entity.dart';
import '../../../tables/presentation/providers/tables_controller_provider.dart';
import '../../domain/entities/tab_entity.dart';
import '../../domain/entities/tab_item_entity.dart';
import '../providers/tab_items_controller_provider.dart';
import '../providers/tabs_controller_provider.dart';
import 'tab_details_page.dart';

class TabDetailsPageState extends ConsumerState<TabDetailsPage> {
  @override
  void initState() {
    super.initState();
    Future.microtask(() {
      ref.read(TabItemsControllerProvider.provider.notifier).load(tabId: widget.tab.id);
    });
  }

  @override
  Widget build(BuildContext context) {
    final itemsAsync = ref.watch(TabItemsControllerProvider.provider);
    final items = itemsAsync.asData?.value;
    final itemsCount = items?.length ?? 0;

    return Scaffold(
      appBar: AppBar(
        title: Text(widget.tab.name?.trim().isNotEmpty == true ? widget.tab.name! : 'Comanda'),
        actions: [
          IconButton(
            onPressed: () => ref.read(TabItemsControllerProvider.provider.notifier).refresh(),
            icon: const Icon(Icons.refresh),
          ),
          IconButton(
            onPressed: widget.tab.isOpen ? _showEditDialog : null,
            icon: const Icon(Icons.edit),
          ),
          IconButton(
            onPressed: widget.tab.isOpen ? _confirmDelete : null,
            icon: const Icon(Icons.delete_outline),
          ),
        ],
      ),
      body: Container(
        color: AppColors.surface,
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
              child: _buildHeader(
                context: context,
                table: widget.table,
                tab: widget.tab,
                itemsCount: itemsCount,
              ),
            ),
            Expanded(
              child: itemsAsync.when(
                data: (items) {
                  if (items.isEmpty) {
                    return const Center(
                      child: Text(
                        'Nenhum item na comanda',
                        style: TextStyle(color: AppColors.textSecondary),
                      ),
                    );
                  }

                  return ListView.separated(
                    padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
                    itemCount: items.length,
                    separatorBuilder: (_, index) => const SizedBox(height: 12),
                    itemBuilder: (context, index) {
                      final item = items[index];
                      final price = item.subtotal.toStringAsFixed(2);
                      return Card(
                        child: ListTile(
                          title: Text(item.productName),
                          subtitle: Text(
                            'Qtd: ${item.quantity}${item.notes?.trim().isNotEmpty == true ? '\n${item.notes}' : ''}',
                          ),
                          trailing: Text('R\$ $price'),
                        ),
                      );
                    },
                  );
                },
                error: (error, _) => Center(
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Text(
                      error is Failure ? error.message : error.toString(),
                      textAlign: TextAlign.center,
                      style: const TextStyle(color: AppColors.textSecondary),
                    ),
                  ),
                ),
                loading: () => const Center(child: CircularProgressIndicator()),
              ),
            ),
          ],
        ),
      ),
      bottomNavigationBar: itemsAsync.hasValue
          ? _buildFooter(total: _sum(items))
          : null,
      floatingActionButton: widget.tab.isOpen
          ? FloatingActionButton.extended(
              onPressed: () async {
                await Navigator.of(context).push(
                  MaterialPageRoute(
                    builder: (_) => CatalogPage(
                      table: widget.table,
                      tab: widget.tab,
                    ),
                  ),
                );
                if (!mounted) return;
                ref.read(TabItemsControllerProvider.provider.notifier).refresh();
              },
              icon: const Icon(Icons.add),
              label: const Text('Adicionar itens'),
            )
          : null,
    );
  }

  Widget _buildHeader({
    required BuildContext context,
    required TableEntity table,
    required TabEntity tab,
    required int itemsCount,
  }) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(table.name, style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 4),
                  Text(
                    tab.isOpen ? 'Aberta' : 'Fechada',
                    style: const TextStyle(color: AppColors.textSecondary),
                  ),
                ],
              ),
            ),
            Text(
              '$itemsCount itens',
              style: const TextStyle(color: AppColors.textSecondary),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildFooter({required double total}) {
    return SafeArea(
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: const BoxDecoration(
          border: Border(
            top: BorderSide(color: AppColors.border),
          ),
        ),
        child: Row(
          children: [
            const Expanded(child: Text('Total parcial')),
            Text('R\$ ${total.toStringAsFixed(2)}'),
          ],
        ),
      ),
    );
  }

  double _sum(List<TabItemEntity>? items) {
    var total = 0.0;
    for (final item in items ?? const <TabItemEntity>[]) {
      total += item.subtotal;
    }
    return total;
  }

  Future<void> _confirmDelete() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Excluir comanda?'),
        content: const Text('Essa ação não pode ser desfeita.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancelar'),
          ),
          FilledButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text('Excluir'),
          ),
        ],
      ),
    );
    if (confirmed != true || !mounted) return;

    final succeeded = await ref
        .read(TabsControllerProvider.provider.notifier)
        .deleteTab(tabId: widget.tab.id);
    if (succeeded) {
      if (!mounted) return;
      Navigator.of(context).pop(true);
    }
  }

  Future<void> _showEditDialog() async {
    final initialName = widget.tab.name ?? '';
    final nameController = TextEditingController(text: initialName);
    var selectedTableId = widget.table.id;

    final decision = await showDialog<({String? name, String? tableId})?>(
      context: context,
      builder: (context) {
        return AlertDialog(
          title: const Text('Editar comanda'),
          content: SizedBox(
            width: 420,
            child: StatefulBuilder(
              builder: (context, setState) {
                return Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    TextField(
                      controller: nameController,
                      decoration: const InputDecoration(
                        labelText: 'Nome',
                        hintText: 'Opcional (ex: João, Família)',
                      ),
                    ),
                    const SizedBox(height: 16),
                    FutureBuilder<List<TableEntity>>(
                      future: ref.read(TablesControllerProvider.provider.future),
                      builder: (context, snapshot) {
                        final tables = snapshot.data ?? const <TableEntity>[];
                        final active = tables.where((t) => t.isActive).toList(growable: false);
                        final display = active.isEmpty ? tables : active;
                        if (display.isNotEmpty && !display.any((t) => t.id == selectedTableId)) {
                          selectedTableId = display.first.id;
                        }

                        return DropdownButtonFormField<String>(
                          key: ValueKey(selectedTableId),
                          initialValue: selectedTableId,
                          decoration: const InputDecoration(labelText: 'Mesa'),
                          items: display
                              .map(
                                (t) => DropdownMenuItem(
                                  value: t.id,
                                  child: Text(t.name),
                                ),
                              )
                              .toList(growable: false),
                          onChanged: snapshot.connectionState == ConnectionState.done
                              ? (value) => setState(() {
                                    if (value != null) selectedTableId = value;
                                  })
                              : null,
                        );
                      },
                    ),
                  ],
                );
              },
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(null),
              child: const Text('Cancelar'),
            ),
            FilledButton(
              onPressed: () => Navigator.of(context).pop(
                (name: nameController.text, tableId: selectedTableId),
              ),
              child: const Text('Salvar'),
            ),
          ],
        );
      },
    );

    nameController.dispose();
    if (decision == null || !mounted) return;

    final trimmedName = decision.name?.trim();
    final newName = trimmedName != null && trimmedName.isNotEmpty ? trimmedName : null;
    final newTableId = decision.tableId?.trim().isNotEmpty == true ? decision.tableId : null;

    final succeeded = await ref.read(TabsControllerProvider.provider.notifier).updateTab(
          tabId: widget.tab.id,
          name: newName,
          tableId: newTableId,
        );

    if (!mounted) return;
    if (!succeeded) return;
    if (newTableId != null && newTableId != widget.table.id) {
      Navigator.of(context).pop(true);
      return;
    }
    Navigator.of(context).pop(true);
  }
}
