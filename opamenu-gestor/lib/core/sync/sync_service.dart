import 'dart:async';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../local_db/app_database.dart';
import 'connectivity_monitor.dart';

part 'sync_service.g.dart';

enum SyncStatus { idle, syncing, error }

@riverpod
class SyncService extends _$SyncService {
  Timer? _syncTimer;

  @override
  AsyncValue<SyncStatus> build() {
    _startPeriodicSync();
    
    // Listen to connectivity changes to trigger sync immediately when back online
    ref.listen<bool>(connectivityMonitorProvider, (previous, isOnline) {
      if (isOnline && state.value == SyncStatus.idle) {
        syncPendingOrders();
      }
    });

    ref.onDispose(() {
      _syncTimer?.cancel();
    });
    return const AsyncValue.data(SyncStatus.idle);
  }

  void _startPeriodicSync() {
    _syncTimer = Timer.periodic(
      const Duration(seconds: 30),
      (_) => syncPendingOrders(),
    );
  }

  Future<void> syncPendingOrders() async {
    final isOnline = ref.read(connectivityMonitorProvider);
    if (!isOnline) return;

    if (state.value == SyncStatus.syncing) return;
    
    state = const AsyncValue.data(SyncStatus.syncing);

    try {
      // 1. Fetch pending orders from SQLite
      // 2. Iterate and send via API
      // 3. Mark as synced
      // For now, logic is a placeholder as per plan
      
      state = const AsyncValue.data(SyncStatus.idle);
    } catch (e, st) {
      state = AsyncValue.error(e, st);
    }
  }
}
