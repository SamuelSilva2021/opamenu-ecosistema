import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/app_colors.dart';
import '../../../../core/domain/failures/failure.dart';
import '../../../auth/presentation/providers/auth_controller_provider.dart';
import '../providers/tables_controller_provider.dart';
import '../widgets/tables_error_state.dart';
import '../widgets/tables_list.dart';

class TablesPage extends ConsumerWidget {
  const TablesPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final tablesAsync = ref.watch(TablesControllerProvider.provider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Mesas'),
        actions: [
          IconButton(
            onPressed: () => ref.read(TablesControllerProvider.provider.notifier).refresh(),
            icon: const Icon(Icons.refresh),
          ),
          IconButton(
            onPressed: () => ref.read(AuthControllerProvider.provider.notifier).signOut(),
            icon: const Icon(Icons.logout),
          ),
        ],
      ),
      body: Container(
        color: AppColors.surface,
        child: tablesAsync.when(
          data: (tables) => TablesList(tables: tables),
          error: (error, _) => TablesErrorState(
            message: error is Failure ? error.message : error.toString(),
          ),
          loading: () => const Center(child: CircularProgressIndicator()),
        ),
      ),
    );
  }
}
