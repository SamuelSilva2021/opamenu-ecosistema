import '../../domain/entities/tab_entity.dart';

class TabModel {
  final String id;
  final String tableId;
  final String? name;
  final int status;
  final DateTime openedAt;
  final DateTime? closedAt;

  const TabModel({
    required this.id,
    required this.tableId,
    required this.name,
    required this.status,
    required this.openedAt,
    required this.closedAt,
  });

  factory TabModel.fromJson(Map<String, Object?> json) {
    return TabModel(
      id: _readString(json, 'id', 'Id'),
      tableId: _readString(json, 'tableId', 'TableId'),
      name: _readStringOrNull(json, 'name', 'Name'),
      status: _parseStatus(json['status'] ?? json['Status']),
      openedAt: DateTime.tryParse(_readString(json, 'openedAt', 'OpenedAt')) ?? DateTime.now(),
      closedAt: DateTime.tryParse(_readString(json, 'closedAt', 'ClosedAt')),
    );
  }

  TabEntity toEntity() {
    return TabEntity(
      id: id,
      tableId: tableId,
      name: name,
      status: status,
      openedAt: openedAt,
      closedAt: closedAt,
    );
  }

  static int _parseStatus(Object? value) {
    if (value is int) return value;
    final asString = value?.toString().toLowerCase();
    if (asString == 'open') return 1;
    if (asString == 'closed') return 2;
    return 0;
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

  static String? _readStringOrNull(
    Map<String, Object?> json,
    String camelCase,
    String pascalCase,
  ) {
    final value = json[camelCase] ?? json[pascalCase];
    if (value == null) return null;
    if (value is String) return value;
    return value.toString();
  }
}
