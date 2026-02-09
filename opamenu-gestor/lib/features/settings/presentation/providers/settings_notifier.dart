
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

part 'settings_notifier.g.dart';

@riverpod
class SettingsNotifier extends _$SettingsNotifier {
  final _storage = const FlutterSecureStorage();
  
  static const _kPrinterKitchen = 'printer_kitchen_ip';
  static const _kPrinterCounter = 'printer_counter_ip';
  static const _kRestaurantName = 'restaurant_name';
  static const _kRestaurantAddress = 'restaurant_address';
  static const _kRestaurantPhone = 'restaurant_phone';

  @override
  FutureOr<Map<String, String>> build() async {
    final kitchenIp = await _storage.read(key: _kPrinterKitchen) ?? '192.168.1.100';
    final counterIp = await _storage.read(key: _kPrinterCounter) ?? '192.168.1.200';
    final name = await _storage.read(key: _kRestaurantName) ?? 'OPAMENU';
    final address = await _storage.read(key: _kRestaurantAddress) ?? 'Rua Principal, 123';
    final phone = await _storage.read(key: _kRestaurantPhone) ?? '(00) 0000-0000';
    
    return {
      'kitchen': kitchenIp,
      'counter': counterIp,
      'name': name,
      'address': address,
      'phone': phone,
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

  Future<void> updateRestaurantInfo({
    required String name,
    required String address,
    required String phone,
  }) async {
    state = const AsyncValue.loading();
    
    try {
      await _storage.write(key: _kRestaurantName, value: name);
      await _storage.write(key: _kRestaurantAddress, value: address);
      await _storage.write(key: _kRestaurantPhone, value: phone);
      
      // Reload state
      ref.invalidateSelf();
    } catch (e) {
      state = AsyncValue.error(e, StackTrace.current);
    }
  }
}
