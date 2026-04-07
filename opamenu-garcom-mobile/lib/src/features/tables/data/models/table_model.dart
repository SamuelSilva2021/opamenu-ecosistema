import '../../domain/entities/table_entity.dart';

class TableModel {
  final String id;
  final String name;
  final bool isActive;

  const TableModel({
    required this.id,
    required this.name,
    required this.isActive,
  });

  factory TableModel.fromJson(Map<String, Object?> json) {
    return TableModel(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      isActive: json['isActive'] is bool ? json['isActive'] as bool : true,
    );
  }

  TableEntity toEntity() {
    return TableEntity(
      id: id,
      name: name,
      isActive: isActive,
    );
  }
}

