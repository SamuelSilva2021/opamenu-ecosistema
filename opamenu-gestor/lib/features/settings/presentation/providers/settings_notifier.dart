
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

part 'settings_notifier.g.dart';

@riverpod
class SettingsNotifier extends _$SettingsNotifier {
  final _storage = const FlutterSecureStorage();
  
  static const _kPrinterKitchen = 'printer_kitchen_ip';
  static const _kPrinterCounter = 'printer_counter_ip';

  @override
  FutureOr<Map<String, String>> build() async {
    final kitchenIp = await _storage.read(key: _kPrinterKitchen) ?? '192.168.1.100';
    final counterIp = await _storage.read(key: _kPrinterCounter) ?? '192.168.1.200';
    
    return {
      'kitchen': kitchenIp,
      'counter': counterIp,
    };
  }

  Future<void> updatePrinterIp(String type, String ip) async {
    state = const AsyncValue.loading();
    
    try {
      if (type == 'kitchen') {
        await _storage.write(key: _kPrinterKitchen, value: ip);
      } else {
        await _storage.write(key: _kPrinterCounter, value: ip);
      }
      
      // Reload state
      ref.invalidateSelf();
    } catch (e) {
      state = AsyncValue.error(e, StackTrace.current);
    }
  }
}
