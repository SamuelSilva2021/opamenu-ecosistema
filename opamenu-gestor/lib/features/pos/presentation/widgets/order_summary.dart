import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:opamenu_gestor/core/constants/app_strings.dart';
import 'package:opamenu_gestor/features/tables/presentation/controllers/tables_controller.dart';
import '../../../../core/theme/app_colors.dart';
import '../../domain/models/cart_item_model.dart';
import '../../domain/models/create_order_request_dto.dart';
import '../controllers/orders_controller.dart';
import '../providers/active_order_provider.dart';
import '../providers/cart_notifier.dart';

class OrderSummary extends ConsumerWidget {
  const OrderSummary({super.key});

  Future<void> _submitTableOrder(BuildContext context, WidgetRef ref, int tableId, List<CartItemModel> cartItems) async {
    if (cartItems.isEmpty) return;

    final activeOrder = ref.read(activeOrderProvider);
    
    final itemsDto = cartItems.map((item) => CreateOrderItemRequestDto(
      productId: item.product.id,
      quantity: item.quantity,
      notes: item.notes,
      addons: [],
    )).toList();

    try {
      if (activeOrder != null) {
         final updatedOrder = await ref.read(ordersControllerProvider.notifier).addItemsToOrder(activeOrder.id, itemsDto);
         if (context.mounted) {
           ref.read(activeOrderProvider.notifier).setOrder(updatedOrder);
         }
      } else {
         final dto = CreateOrderRequestDto(
           customerName: "Mesa $tableId",
           customerPhone: "0000000000",
           isDelivery: false,
           items: itemsDto,
           tableId: tableId,
           orderType: 2, // Table
         );
         
         final newOrder = await ref.read(ordersControllerProvider.notifier).createOrder(dto);
         if (context.mounted) {
           ref.read(activeOrderProvider.notifier).setOrder(newOrder);
         }
      }
      
      if (context.mounted) {
        ref.read(cartProvider.notifier).clearCart();
        ScaffoldMessenger.of(context).showSnackBar(const SnackBar(content: Text('Pedido atualizado com sucesso!')));
      }
    } catch (e) {
      if (context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text('Erro ao atualizar pedido: $e')));
      }
    }
  }

  Future<void> _closeAccount(BuildContext context, WidgetRef ref, int tableId) async {
    try {
      final order = await ref.read(tablesControllerProvider.notifier).closeAccount(tableId);
      if (context.mounted) {
        // Show total and clear
        showDialog(
          context: context, 
          builder: (context) => AlertDialog(
            title: const Text('Conta Fechada'),
            content: Text('Total a pagar: R\$ ${order.total.toStringAsFixed(2)}'),
            actions: [
              TextButton(
                onPressed: () {
                  Navigator.pop(context);
                  ref.read(activeTableProvider.notifier).clear();
                  ref.read(activeOrderProvider.notifier).clear();
                  context.go('/tables');
                }, 
                child: const Text('OK')
              )
            ],
          )
        );
      }
    } catch (e) {
      if (context.mounted) {
        ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text('Erro ao fechar conta: $e')));
      }
    }
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final cartItems = ref.watch(cartProvider);
    final activeTableId = ref.watch(activeTableProvider);
    final activeOrder = ref.watch(activeOrderProvider);
    
    double subtotal = cartItems.fold(0, (sum, item) => sum + item.totalPrice);
    double tax = subtotal * 0.08; // 8% tax mock
    double charges = 0; // Service charges mock
    double total = subtotal + tax + charges;

    return Container(
      width: 350,
      color: Colors.white,
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                activeTableId != null ? 'Mesa #$activeTableId' : '${AppStrings.orderNumber} 256482',
                style: const TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                  color: AppColors.textPrimary,
                ),
              ),
              IconButton(
                icon: const Icon(Icons.close),
                onPressed: () {
                   if (activeTableId != null) {
                     ref.read(activeTableProvider.notifier).clear();
                     ref.read(activeOrderProvider.notifier).clear();
                     context.go('/tables');
                   }
                },
              ),
            ],
          ),
          const SizedBox(height: 24),
          
          if (activeOrder != null) ...[
             const Text('Itens já pedidos:', style: TextStyle(fontWeight: FontWeight.bold, color: Colors.grey)),
             const SizedBox(height: 8),
             Expanded(
               flex: 1,
               child: ListView.separated(
                 itemCount: activeOrder.items.length,
                 separatorBuilder: (_, __) => const Divider(height: 8),
                 itemBuilder: (context, index) {
                   final item = activeOrder.items[index];
                   return Row(
                     mainAxisAlignment: MainAxisAlignment.spaceBetween,
                     children: [
                        Expanded(child: Text(item.productName, style: const TextStyle(fontSize: 14))),
                        Text('x${item.quantity}', style: const TextStyle(fontSize: 14)),
                        const SizedBox(width: 8),
                        Text('R\$ ${item.subtotal.toStringAsFixed(2)}', style: const TextStyle(fontSize: 14, fontWeight: FontWeight.bold)),
                     ],
                   );
                 }
               ),
             ),
             const Divider(thickness: 2),
             const SizedBox(height: 8),
          ],

          if (activeTableId != null) 
            const Text('Novos itens:', style: TextStyle(fontWeight: FontWeight.bold, color: Colors.green)),

          // Cart Items List
          Expanded(
            flex: 2,
            child: cartItems.isEmpty
                ? const Center(child: Text(AppStrings.noItemsInCart))
                : ListView.separated(
                    itemCount: cartItems.length,
                    separatorBuilder: (_, __) => const SizedBox(height: 16),
                    itemBuilder: (context, index) {
                      final item = cartItems[index];
                      return Row(
                        children: [
                          Container(
                            width: 60,
                            height: 60,
                            decoration: BoxDecoration(
                              borderRadius: BorderRadius.circular(8),
                              image: item.product.imageUrl != null
                                  ? DecorationImage(
                                      image: NetworkImage(item.product.imageUrl!),
                                      fit: BoxFit.cover,
                                    )
                                  : null,
                              color: item.product.imageUrl == null ? Colors.grey[200] : null,
                            ),
                            child: item.product.imageUrl == null
                                ? const Icon(Icons.image_not_supported, size: 20, color: Colors.grey)
                                : null,
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  item.product.name,
                                  style: const TextStyle(
                                    fontWeight: FontWeight.bold,
                                    fontSize: 14,
                                  ),
                                  maxLines: 1,
                                  overflow: TextOverflow.ellipsis,
                                ),
                                Text(
                                  'R\$ ${item.totalPrice.toStringAsFixed(2)}',
                                  style: const TextStyle(
                                    color: AppColors.textSecondary,
                                    fontSize: 12,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          Row(
                            children: [
                              IconButton(
                                icon: const Icon(Icons.remove_circle_outline, size: 20),
                                onPressed: () {
                                  ref.read(cartProvider.notifier).removeProduct(item.product);
                                },
                                padding: EdgeInsets.zero,
                                constraints: const BoxConstraints(),
                              ),
                              Padding(
                                padding: const EdgeInsets.symmetric(horizontal: 8),
                                child: Text(
                                  '${item.quantity}',
                                  style: const TextStyle(fontWeight: FontWeight.bold),
                                ),
                              ),
                              IconButton(
                                icon: const Icon(Icons.add_circle, color: AppColors.primary, size: 20),
                                onPressed: () {
                                  ref.read(cartProvider.notifier).addProduct(item.product);
                                },
                                padding: EdgeInsets.zero,
                                constraints: const BoxConstraints(),
                              ),
                            ],
                          ),
                        ],
                      );
                    },
                  ),
          ),
          const Divider(height: 32),
          // Totals
          if (activeTableId == null) ...[
            _SummaryRow(label: AppStrings.subtotal, value: subtotal),
            const SizedBox(height: 8),
            _SummaryRow(label: AppStrings.deliveryFee, value: tax),
            const SizedBox(height: 8),
            _SummaryRow(label: AppStrings.discount, value: charges),
            const Divider(height: 32),
          ],
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                activeTableId != null ? 'Total Novos Itens' : AppStrings.total,
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  fontSize: 18,
                  color: AppColors.textPrimary,
                ),
              ),
              Text(
                '\$${total.toStringAsFixed(2)}',
                style: const TextStyle(
                  fontWeight: FontWeight.bold,
                  fontSize: 18,
                  color: AppColors.textPrimary,
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),
          
          if (activeTableId != null) ...[
             SizedBox(
               width: double.infinity,
               child: ElevatedButton(
                 onPressed: cartItems.isNotEmpty 
                    ? () => _submitTableOrder(context, ref, activeTableId, cartItems)
                    : null,
                 style: ElevatedButton.styleFrom(
                   backgroundColor: Colors.green,
                   foregroundColor: Colors.white,
                   padding: const EdgeInsets.symmetric(vertical: 16),
                   shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                 ),
                 child: const Text('Enviar para Cozinha', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
               ),
             ),
             const SizedBox(height: 12),
             SizedBox(
               width: double.infinity,
               child: OutlinedButton(
                 onPressed: activeOrder != null 
                    ? () => _closeAccount(context, ref, activeTableId)
                    : null,
                 style: OutlinedButton.styleFrom(
                   padding: const EdgeInsets.symmetric(vertical: 16),
                   side: const BorderSide(color: Colors.red),
                   shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                 ),
                 child: const Text('Fechar Conta', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.red)),
               ),
             ),
          ] else
          SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              onPressed: cartItems.isNotEmpty
                  ? () {
                      context.push('/checkout');
                    }
                  : null,
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.primary,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(vertical: 16),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
              ),
              child: const Text(
                AppStrings.placeOrder,
                style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _SummaryRow extends StatelessWidget {
  final String label;
  final double value;

  const _SummaryRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(
          label,
          style: const TextStyle(color: AppColors.textSecondary),
        ),
        Text(
          '\$${value.toStringAsFixed(2)}',
          style: const TextStyle(fontWeight: FontWeight.bold),
        ),
      ],
    );
  }
}
