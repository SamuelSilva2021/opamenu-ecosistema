import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/app_colors.dart';

class OrdersHomePage extends ConsumerWidget {
  const OrdersHomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    const mesas = <int>[1, 2, 3, 4, 5, 6];

    return Scaffold(
      appBar: AppBar(title: const Text('Mesas')),
      body: Container(
        color: AppColors.surface,
        child: ListView.separated(
          padding: const EdgeInsets.all(16),
          itemCount: mesas.length,
          separatorBuilder: (_, index) => const SizedBox(height: 12),
          itemBuilder: (context, index) {
            final mesa = mesas[index];

            return Card(
              child: ListTile(
                title: Text('Mesa $mesa'),
                subtitle: const Text('Toque para iniciar um pedido'),
                trailing: const Icon(Icons.chevron_right),
                onTap: () {},
              ),
            );
          },
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () {},
        icon: const Icon(Icons.add),
        label: const Text('Novo pedido'),
      ),
    );
  }
}
