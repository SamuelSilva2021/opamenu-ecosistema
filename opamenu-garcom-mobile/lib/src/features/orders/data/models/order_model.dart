import '../../domain/entities/order_entity.dart';

class OrderModel {
  final String id;
  final int orderNumber;

  const OrderModel({
    required this.id,
    required this.orderNumber,
  });

  factory OrderModel.fromJson(Map<String, Object?> json) {
    return OrderModel(
      id: json['id']?.toString() ?? '',
      orderNumber: json['orderNumber'] is int ? json['orderNumber'] as int : 0,
    );
  }

  OrderEntity toEntity() {
    return OrderEntity(
      id: id,
      orderNumber: orderNumber,
    );
  }
}

