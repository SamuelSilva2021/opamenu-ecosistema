import 'package:json_annotation/json_annotation.dart';
import '../enums/order_status.dart';
import 'order_item_response_dto.dart';

part 'order_response_dto.g.dart';

@JsonSerializable()
class OrderResponseDto {
  final String id;
  final String customerName;
  final String customerPhone;
  final String? customerEmail;
  final String? deliveryAddress;
  final double subtotal;
  final double deliveryFee;
  final double discountAmount;
  final String? couponCode;
  final double total;
  final OrderStatus status;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final bool isDelivery;
  final String? notes;
  final int? estimatedPreparationMinutes;
  final DateTime? estimatedDeliveryTime;
  final int queuePosition;
  final List<OrderItemResponseDto> items;

  OrderResponseDto({
    required this.id,
    required this.customerName,
    required this.customerPhone,
    this.customerEmail,
    this.deliveryAddress,
    required this.subtotal,
    required this.deliveryFee,
    required this.discountAmount,
    this.couponCode,
    required this.total,
    required this.status,
    required this.createdAt,
    this.updatedAt,
    required this.isDelivery,
    this.notes,
    this.estimatedPreparationMinutes,
    this.estimatedDeliveryTime,
    required this.queuePosition,
    this.items = const [],
  });

  factory OrderResponseDto.fromJson(Map<String, dynamic> json) =>
      _$OrderResponseDtoFromJson(json);

  Map<String, dynamic> toJson() => _$OrderResponseDtoToJson(this);
}
