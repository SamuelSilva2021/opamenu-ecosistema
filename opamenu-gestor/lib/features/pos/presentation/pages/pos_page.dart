import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/core/constants/app_strings.dart';
import '../../../../core/theme/app_colors.dart';
import '../providers/product_provider.dart';
import '../widgets/product_card.dart';
import '../widgets/order_summary.dart';

class PosPage extends ConsumerWidget {
  const PosPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return LayoutBuilder(
      builder: (context, constraints) {
        // Se houver espaço suficiente (ex: > 800px), mostramos o layout dividido.
        // Caso contrário, mostramos apenas os produtos e um botão flutuante para o carrinho.
        final isLargeScreen = constraints.maxWidth >= 800;

        if (isLargeScreen) {
          return Row(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // Main Content
              Expanded(
                child: _buildContent(context, ref),
              ),
              // Right Panel - Order Summary
              const OrderSummary(),
            ],
          );
        } else {
          // Mobile/Tablet Layout
          return Scaffold(
            body: _buildContent(context, ref),
            floatingActionButton: FloatingActionButton.extended(
              backgroundColor: AppColors.primary,
              onPressed: () {
                showModalBottomSheet(
                  context: context,
                  isScrollControlled: true,
                  backgroundColor: Colors.transparent,
                  builder: (context) => Container(
                    height: MediaQuery.of(context).size.height * 0.85,
                    decoration: const BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
                    ),
                    child: const ClipRRect(
                      borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
                      child: OrderSummary(),
                    ),
                  ),
                );
              },
              icon: const Icon(Icons.shopping_cart, color: Colors.white),
              label: const Text('Ver Pedido', style: TextStyle(color: Colors.white)),
            ),
          );
        }
      },
    );
  }

  Widget _buildContent(BuildContext context, WidgetRef ref) {
    final productsAsync = ref.watch(filteredProductsProvider);
    final isMobile = MediaQuery.of(context).size.width < 800;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        // Top Bar
        Container(
          padding: EdgeInsets.symmetric(horizontal: isMobile ? 16 : 24, vertical: 16),
          color: Colors.white,
          child: Row(
            children: [
              if (!isMobile) ...[
                const Text(
                  AppStrings.appName,
                  style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                    color: AppColors.textPrimary,
                  ),
                ),
                const SizedBox(width: 48),
              ],
              Expanded(
                child: TextField(
                  onChanged: (value) {
                    ref.read(productSearchQueryProvider.notifier).setQuery(value);
                  },
                  decoration: InputDecoration(
                    hintText: AppStrings.searchProduct,
                    prefixIcon: const Icon(Icons.search),
                    border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(12),
                      borderSide: BorderSide.none,
                    ),
                    filled: true,
                    fillColor: const Color(0xFFF5F5F5),
                    contentPadding: const EdgeInsets.symmetric(horizontal: 16),
                  ),
                ),
              ),
              if (!isMobile) ...[
                const SizedBox(width: 24),
                IconButton(
                  onPressed: () {},
                  icon: const Icon(Icons.notifications_none),
                ),
                const SizedBox(width: 16),
                const CircleAvatar(
                  backgroundColor: AppColors.primary,
                  child: Icon(Icons.person, color: Colors.white),
                ),
              ] else ...[
                 const SizedBox(width: 12),
                 const CircleAvatar(
                   radius: 18,
                   backgroundColor: AppColors.primary,
                   child: Icon(Icons.person, size: 20, color: Colors.white),
                 ),
              ]
            ],
          ),
        ),
        // Scrollable Content
        Expanded(
          child: Padding(
            padding: EdgeInsets.all(isMobile ? 16 : 24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                const Text(
                  AppStrings.specialMenuForYou,
                  style: TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                    color: AppColors.textPrimary,
                  ),
                ),
                const SizedBox(height: 24),
                Expanded(
                  child: productsAsync.when(
                    data: (products) {
                      if (products.isEmpty) {
                        return const Center(child: Text(AppStrings.noProductsFound));
                      }
                      return GridView.builder(
                        gridDelegate: SliverGridDelegateWithMaxCrossAxisExtent(
                          maxCrossAxisExtent: 300,
                          childAspectRatio: 0.75,
                          crossAxisSpacing: isMobile ? 16 : 24,
                          mainAxisSpacing: isMobile ? 16 : 24,
                        ),
                        itemCount: products.length,
                        itemBuilder: (context, index) {
                          return ProductCard(product: products[index]);
                        },
                      );
                    },
                    loading: () {
                      return const Center(child: CircularProgressIndicator());
                    },
                    error: (error, stack) {
                      return Center(child: Text('Error: $error'));
                    },
                  ),
                ),
              ],
            ),
          ),
        ),
      ],
    );
  }
}
