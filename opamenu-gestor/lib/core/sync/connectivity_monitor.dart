import 'dart:async';
import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

part 'connectivity_monitor.g.dart';

@riverpod
class ConnectivityMonitor extends _$ConnectivityMonitor {
  StreamSubscription<List<ConnectivityResult>>? _subscription;

  @override
  bool build() {
    _init();
    ref.onDispose(() {
      _subscription?.cancel();
    });
    return true; // Assumes online initially
  }

  void _init() {
    _subscription = Connectivity().onConnectivityChanged.listen((List<ConnectivityResult> result) {
      final isOnline = !result.contains(ConnectivityResult.none);
      if (state != isOnline) {
        state = isOnline;
      }
    });
    Connectivity().checkConnectivity().then((List<ConnectivityResult> result) {
      state = !result.contains(ConnectivityResult.none);
    });
  }
}
