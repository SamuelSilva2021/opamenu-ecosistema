import 'dart:convert';

class JsonMapReader {
  final Map<String, Object?> map;

  const JsonMapReader(this.map);

  Map<String, Object?>? mapOrNull(String key) {
    final value = map[key];
    if (value is Map<String, Object?>) return value;
    if (value is Map) return value.cast<String, Object?>();
    return null;
  }

  List<Object?> listOrEmpty(String key) {
    final value = map[key];
    if (value is List<Object?>) return value;
    if (value is List) return value.cast<Object?>();
    return const [];
  }

  String string(String key, {String defaultValue = ''}) {
    final value = map[key];
    if (value == null) return defaultValue;
    if (value is String) return value;
    return value.toString();
  }

  String? stringOrNull(String key) {
    final value = map[key];
    if (value == null) return null;
    if (value is String) return value;
    return value.toString();
  }

  int intValue(String key, {int defaultValue = 0}) {
    final value = map[key];
    if (value is int) return value;
    if (value is num) return value.toInt();
    if (value is String) return int.tryParse(value) ?? defaultValue;
    return defaultValue;
  }

  bool boolValue(String key, {bool defaultValue = false}) {
    final value = map[key];
    if (value is bool) return value;
    if (value is String) return value.toLowerCase() == 'true';
    if (value is num) return value != 0;
    return defaultValue;
  }

  String stringifyUnknown(Object? value) {
    if (value == null) return '';
    if (value is String) return value;
    return jsonEncode(value);
  }
}

