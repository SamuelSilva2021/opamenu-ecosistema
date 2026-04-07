import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

class CatalogSearchQueryProvider {
  static final Provider<ValueNotifier<String>> provider = Provider.autoDispose((ref) {
    final notifier = ValueNotifier<String>('');
    ref.onDispose(notifier.dispose);
    return notifier;
  });
}
