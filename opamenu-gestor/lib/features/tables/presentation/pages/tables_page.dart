import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:opamenu_gestor/core/presentation/widgets/permission_gate.dart';
import 'package:opamenu_gestor/features/pos/presentation/providers/active_order_provider.dart';
import 'package:opamenu_gestor/features/pos/presentation/providers/cart_notifier.dart';
import 'package:qr_flutter/qr_flutter.dart';
import '../controllers/tables_controller.dart';
import '../widgets/table_form_dialog.dart';
import '../../../../core/theme/app_colors.dart';

class TablesPage extends ConsumerWidget {
  const TablesPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final tablesAsync = ref.watch(tablesControllerProvider);
    final currentPage = ref.watch(tablesPaginationProvider);

    return Scaffold(
      backgroundColor: const Color(0xFFF9FAFB),
      body: Padding(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  'Gerenciamento de Mesas',
                  style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                    color: AppColors.textPrimary,
                  ),
                ),
                ElevatedButton.icon(
                  onPressed: () {
                    showDialog(
                      context: context,
                      builder: (context) => const TableFormDialog(),
                    );
                  },
                  icon: const Icon(Icons.add),
                  label: const Text('Nova Mesa'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    foregroundColor: Colors.white,
                    minimumSize: const Size(0, 48),
                    padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 24),

            // Content
            Expanded(
              child: Container(
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
                child: Column(
                  children: [
                    // Table Header
                    const Padding(
                      padding: EdgeInsets.all(16.0),
                      child: Row(
                        children: [
                          Expanded(flex: 1, child: Text('ID', style: TextStyle(fontWeight: FontWeight.bold))),
                          Expanded(flex: 3, child: Text('Nome', style: TextStyle(fontWeight: FontWeight.bold))),
                          Expanded(flex: 2, child: Text('Capacidade', style: TextStyle(fontWeight: FontWeight.bold))),
                          Expanded(flex: 2, child: Text('Status', style: TextStyle(fontWeight: FontWeight.bold))),
                          Expanded(flex: 2, child: Text('Ações', style: TextStyle(fontWeight: FontWeight.bold), textAlign: TextAlign.end)),
                        ],
                      ),
                    ),
                    const Divider(height: 1),

                    // Table List
                    Expanded(
                      child: tablesAsync.when(
                        data: (pagedResponse) {
                          final tables = pagedResponse.data ?? [];
                          if (tables.isEmpty) {
                            return const Center(child: Text('Nenhuma mesa encontrada'));
                          }
                          return ListView.separated(
                            itemCount: tables.length,
                            separatorBuilder: (context, index) => const Divider(height: 1),
                            itemBuilder: (context, index) {
                              final table = tables[index];
                              return Padding(
                                padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 12.0),
                                child: Row(
                                  children: [
                                    Expanded(flex: 1, child: Text('#${table.id}')),
                                    Expanded(flex: 3, child: Text(table.name)),
                                    Expanded(flex: 2, child: Text('${table.capacity} lugares')),
                                    Expanded(
                                      flex: 2,
                                      child: Container(
                                        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                                        decoration: BoxDecoration(
                                          color: (table.isActive ? Colors.green : Colors.red).withValues(alpha: 0.1),
                                          borderRadius: BorderRadius.circular(8),
                                        ),
                                        child: Text(
                                          table.isActive ? 'Ativa' : 'Inativa',
                                          style: TextStyle(
                                            color: table.isActive ? Colors.green : Colors.red,
                                            fontWeight: FontWeight.bold,
                                            fontSize: 12,
                                          ),
                                        ),
                                      ),
                                    ),
                                    Expanded(
                                      flex: 2,
                                      child: Row(
                                        mainAxisAlignment: MainAxisAlignment.end,
                                        children: [
                                          IconButton(
                                            icon: const Icon(Icons.restaurant_menu, color: Colors.purple),
                                            tooltip: 'Pedidos',
                                            onPressed: () async {
                                              // Set Active Table
                                              ref.read(activeTableProvider.notifier).setTableId(table.id);
                                              
                                              // Check active order
                                              final order = await ref.read(tablesControllerProvider.notifier).checkActiveOrder(table.id);
                                              ref.read(activeOrderProvider.notifier).setOrder(order);
                                              
                                              // Clear cart
                                              ref.read(cartProvider.notifier).clearCart();
                                              
                                              if (context.mounted) {
                                                context.go('/pos');
                                              }
                                            },
                                          ),
                                          IconButton(
                                            icon: const Icon(Icons.qr_code, color: Colors.blue),
                                            tooltip: 'Gerar QR Code',
                                            onPressed: () => _showQrCode(context, ref, table.id, table.name),
                                          ),
                                          PermissionGate(
                                            module: 'TABLE',
                                            operation: 'UPDATE',
                                            child: IconButton(
                                              icon: const Icon(Icons.edit, color: Colors.orange),
                                              tooltip: 'Editar',
                                              onPressed: () {
                                                showDialog(
                                                  context: context,
                                                  builder: (context) => TableFormDialog(table: table),
                                                );
                                              },
                                            ),
                                          ),
                                          PermissionGate(
                                            module: 'TABLE',
                                            operation: 'DELETE',
                                            child: IconButton(
                                              icon: const Icon(Icons.delete, color: Colors.red),
                                              tooltip: 'Excluir',
                                              onPressed: () => _confirmDelete(context, ref, table.id),
                                            ),
                                          ),
                                        ],
                                      ),
                                    ),
                                  ],
                                ),
                              );
                            },
                          );
                        },
                        loading: () => const Center(child: CircularProgressIndicator()),
                        error: (error, stack) => Center(child: Text('Erro: $error')),
                      ),
                    ),

                    // Pagination
                    if (tablesAsync.hasValue && tablesAsync.value!.data != null)
                      Container(
                        padding: const EdgeInsets.all(16),
                        decoration: BoxDecoration(
                          border: Border(top: BorderSide(color: Colors.grey[200]!)),
                        ),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Text('Página $currentPage de ${(tablesAsync.value!.totalItems / 10).ceil()}'),
                            Row(
                              children: [
                                IconButton(
                                  onPressed: currentPage > 1
                                      ? () => ref.read(tablesPaginationProvider.notifier).previousPage()
                                      : null,
                                  icon: const Icon(Icons.chevron_left),
                                ),
                                IconButton(
                                  onPressed: currentPage < (tablesAsync.value!.totalItems / 10).ceil()
                                      ? () => ref.read(tablesPaginationProvider.notifier).nextPage()
                                      : null,
                                  icon: const Icon(Icons.chevron_right),
                                ),
                              ],
                            ),
                          ],
                        ),
                      ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _confirmDelete(BuildContext context, WidgetRef ref, String id) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Confirmar exclusão'),
        content: const Text('Tem certeza que deseja excluir esta mesa?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(false),
            child: const Text('Cancelar'),
          ),
          TextButton(
            onPressed: () => Navigator.of(context).pop(true),
            child: const Text('Excluir', style: TextStyle(color: Colors.red)),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      await ref.read(tablesControllerProvider.notifier).deleteTable(id);
    }
  }

  Future<void> _showQrCode(BuildContext context, WidgetRef ref, String id, String tableName) async {
    try {
      // First, get the QR code URL (or string data) from the controller
      final qrData = await ref.read(tablesControllerProvider.notifier).generateQrCode(id);
      
      if (context.mounted) {
        showDialog(
          context: context,
          builder: (context) => AlertDialog(
            title: Text('QR Code - $tableName'),
            content: SizedBox(
              width: 300,
              height: 300,
              child: Center(
                child: QrImageView(
                  data: qrData,
                  version: QrVersions.auto,
                  size: 280.0,
                ),
              ),
            ),
            actions: [
              TextButton(
                onPressed: () => Navigator.of(context).pop(),
                child: const Text('Fechar'),
              ),
            ],
          ),
        );
      }
    } catch (e) {
      if (context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Erro ao gerar QR Code: $e'), backgroundColor: Colors.red),
        );
      }
    }
  }
}
