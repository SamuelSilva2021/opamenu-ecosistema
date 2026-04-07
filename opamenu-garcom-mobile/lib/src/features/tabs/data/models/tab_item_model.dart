import '../../domain/entities/tab_item_entity.dart';

class TabItemModel {
  final String id;
  final String productId;
  final String productName;
  final double unitPrice;
  final int quantity;
  final double subtotal;
  final String? notes;

  const TabItemModel({
    required this.id,
    required this.productId,
    required this.productName,
    required this.unitPrice,
    required this.quantity,
    required this.subtotal,
    required this.notes,
  });

  factory TabItemModel.fromJson(Map<String, Object?> json) {
    return TabItemModel(
      id: _readString(json, 'id', 'Id'),
      productId: _readString(json, 'productId', 'ProductId'),
      productName: _readString(json, 'productName', 'ProductName'),
      unitPrice: _toDouble(json['unitPrice'] ?? json['UnitPrice']),
      quantity: _toInt(json['quantity'] ?? json['Quantity']),
      subtotal: _toDouble(json['subtotal'] ?? json['Subtotal']),
      notes: _readStringOrNull(json, 'notes', 'Notes'),
    );
  }

  TabItemEntity toEntity() {
    return TabItemEntity(
      id: id,
      productId: productId,
      productName: productName,
      unitPrice: unitPrice,
      quantity: quantity,
      subtotal: subtotal,
      notes: notes,
    );
  }

  static double _toDouble(Object? value) {
    if (value is double) return value;
    if (value is int) return value.toDouble();
    return double.tryParse(value?.toString() ?? '') ?? 0;
  }

  static int _toInt(Object? value) {
    if (value is int) return value;
    if (value is num) return value.toInt();
    return int.tryParse(value?.toString() ?? '') ?? 0;
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
