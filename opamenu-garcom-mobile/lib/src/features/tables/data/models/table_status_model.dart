import '../../domain/entities/table_entity.dart';

class TableStatusModel {
  final String id;
  final String name;
  final bool isActive;
  final int openTabsCount;

  const TableStatusModel({
    required this.id,
    required this.name,
    required this.isActive,
    required this.openTabsCount,
  });

  factory TableStatusModel.fromJson(Map<String, Object?> json) {
    final tabs = json['tabs'] ?? json['Tabs'];
    return TableStatusModel(
      id: _readString(json, 'id', 'Id'),
      name: _readString(json, 'name', 'Name'),
      isActive: _readBool(json, 'isActive', 'IsActive', defaultValue: true),
      openTabsCount: _countOpenTabs(tabs),
    );
  }

  TableEntity toEntity() {
    return TableEntity(
      id: id,
      name: name,
      isActive: isActive,
      openTabsCount: openTabsCount,
    );
  }

  static int _countOpenTabs(Object? rawTabs) {
    if (rawTabs is! List) return 0;
    var count = 0;
    for (final item in rawTabs) {
      if (item is! Map) continue;
      final map = item.cast<String, Object?>();
      final status = map['status'] ?? map['Status'];
      if (_isOpenStatus(status)) count++;
    }
    return count;
  }

  static bool _isOpenStatus(Object? value) {
    if (value is int) return value == 1;
    final asString = value?.toString().trim();
    if (asString == null || asString.isEmpty) return false;
    final parsed = int.tryParse(asString);
    if (parsed != null) return parsed == 1;
    return asString.toLowerCase() == 'open';
  }

  static String _readString(
    Map<String, Object?> json,
    String camelCase,
    String pascalCase,
  ) {
    final value = json[camelCase] ?? json[pascalCase];
    if (value == null) return '';
    if (value is String) return value;
    return value.toString();
  }

  static bool _readBool(
    Map<String, Object?> json,
    String camelCase,
    String pascalCase, {
    required bool defaultValue,
  }) {
    final value = json[camelCase] ?? json[pascalCase];
    if (value is bool) return value;
    if (value == null) return defaultValue;
    final normalized = value.toString().toLowerCase();
    if (normalized == 'true') return true;
    if (normalized == 'false') return false;
    return defaultValue;
  }
}
