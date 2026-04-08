import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/app_colors.dart';
import '../../../../app/opamenu_garcom_app.dart';
import '../../../../core/domain/failures/failure.dart';
import '../../../tables/domain/entities/table_entity.dart';
import '../../../tabs/domain/entities/tab_entity.dart';
import '../../../tabs/presentation/providers/add_tab_items_controller_provider.dart';
import '../providers/catalog_controller_provider.dart';
import '../providers/catalog_search_query_provider.dart';
import '../../domain/entities/product_entity.dart';

class CatalogPage extends ConsumerWidget {
  final TableEntity table;
  final TabEntity tab;

  const CatalogPage({
    super.key,
    required this.table,
    required this.tab,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final searchNotifier = ref.watch(CatalogSearchQueryProvider.provider);
    final productsAsync = ref.watch(CatalogControllerProvider.provider);
    final addItemsState = ref.watch(AddTabItemsControllerProvider.provider);

    final title = tab.name?.trim().isNotEmpty == true
        ? '${table.name} • ${tab.name}'
        : '${table.name} • Comanda';

    return Scaffold(
      appBar: AppBar(
        title: Text(title),
        actions: [
          IconButton(
            onPressed: () => ref.read(CatalogControllerProvider.provider.notifier).refresh(),
            icon: const Icon(Icons.refresh),
          ),
        ],
      ),
      body: Container(
        color: AppColors.surface,
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.all(16),
              child: TextField(
                decoration: const InputDecoration(
                  prefixIcon: Icon(Icons.search),
                  hintText: 'Buscar produto',
                  border: OutlineInputBorder(),
                ),
                        onChanged: (value) => searchNotifier.value = value,
              ),
            ),
            if (addItemsState.isLoading) const LinearProgressIndicator(minHeight: 2),
            Expanded(
                      child: ValueListenableBuilder<String>(
                        valueListenable: searchNotifier,
                        builder: (context, query, _) => productsAsync.when(
                          data: (products) => _buildList(
                            context: context,
                            ref: ref,
                            products: _filterProducts(products, query),
                            isBusy: addItemsState.isLoading,
                          ),
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
            ),
          ],
        ),
      ),
    );
  }

  List<ProductEntity> _filterProducts(List<ProductEntity> products, String term) {
    final query = term.trim().toLowerCase();
    if (query.isEmpty) return products.where((p) => p.isActive).toList(growable: false);

    return products
        .where((p) => p.isActive)
        .where(
          (p) =>
              p.name.toLowerCase().contains(query) ||
              p.categoryName.toLowerCase().contains(query),
        )
        .toList(growable: false);
  }

  Widget _buildList({
    required BuildContext context,
    required WidgetRef ref,
    required List<ProductEntity> products,
    required bool isBusy,
  }) {
    if (products.isEmpty) {
      return const Center(child: Text('Nenhum produto encontrado'));
    }

    return ListView.separated(
      padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
      itemCount: products.length,
              separatorBuilder: (_, _) => const SizedBox(height: 12),
      itemBuilder: (context, index) {
        final product = products[index];
        return Card(
          child: ListTile(
            title: Text(product.name),
            subtitle: Text(product.categoryName),
            trailing: Text('R\$ ${product.price.toStringAsFixed(2)}'),
            onTap: isBusy
                ? null
                : () async {
                    final item = await _askItemDetails(context);
                    if (!context.mounted) return;
                    if (item == null) return;

                    final (quantity, notes) = item;
                    final controller = ref.read(
                      AddTabItemsControllerProvider.provider.notifier,
                    );
                    await controller.addItem(
                      productId: product.id,
                      tabId: tab.id,
                      quantity: quantity,
                      notes: notes,
                    );

                    final state =
                        ref.read(AddTabItemsControllerProvider.provider);
                    final messenger =
                        OpaMenuGarcomApp.scaffoldMessengerKey.currentState;
                    if (state.hasError) {
                      final error = state.error;
                      final message = error is Failure
                          ? error.message
                          : 'Falha ao adicionar item';
                      messenger?.showSnackBar(
                        SnackBar(content: Text(message)),
                      );
                      controller.clear();
                      return;
                    }

                    if (state.value == true) {
                      messenger?.showSnackBar(
                        const SnackBar(
                          content: Text('Item adicionado na comanda'),
                        ),
                      );
                      controller.clear();
                    }
                  },
          ),
        );
      },
    );
  }

  Future<(int, String?)?> _askItemDetails(BuildContext context) async {
    var quantityText = '1';
    var notesText = '';
    final result = await showDialog<(int, String?)?>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Adicionar item'),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                initialValue: quantityText,
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(
                  labelText: 'Quantidade',
                  border: OutlineInputBorder(),
                ),
                onChanged: (value) => quantityText = value,
              ),
              const SizedBox(height: 12),
              TextFormField(
                initialValue: notesText,
                decoration: const InputDecoration(
                  labelText: 'Observações (opcional)',
                  border: OutlineInputBorder(),
                ),
                onChanged: (value) => notesText = value,
              ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(null),
            child: const Text('Cancelar'),
          ),
          FilledButton(
            onPressed: () {
              final quantity = int.tryParse(quantityText.trim()) ?? 0;
              if (quantity < 1 || quantity > 99) {
                OpaMenuGarcomApp.scaffoldMessengerKey.currentState?.showSnackBar(
                  const SnackBar(content: Text('Quantidade inválida')),
                );
                return;
              }

              final notes = notesText.trim();
              Navigator.of(context).pop((quantity, notes.isEmpty ? null : notes));
            },
            child: const Text('Adicionar'),
          ),
        ],
      ),
    );
    return result;
  }
}
