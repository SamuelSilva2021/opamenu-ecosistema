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
  final int? orderNumber;
  final List<OrderItemResponseDto> items;
  final String? driverName;

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
    this.orderNumber,
    this.items = const [],
    this.driverName,
  });

  factory OrderResponseDto.fromJson(Map<String, dynamic> json) =>
      _$OrderResponseDtoFromJson(json);

  Map<String, dynamic> toJson() => _$OrderResponseDtoToJson(this);

  OrderResponseDto copyWith({
    String? id,
    String? customerName,
    String? customerPhone,
    String? customerEmail,
    String? deliveryAddress,
    double? subtotal,
    double? deliveryFee,
    double? discountAmount,
    String? couponCode,
    double? total,
    OrderStatus? status,
    DateTime? createdAt,
    DateTime? updatedAt,
    bool? isDelivery,
    String? notes,
    int? estimatedPreparationMinutes,
    DateTime? estimatedDeliveryTime,
    int? queuePosition,
    int? orderNumber,
    List<OrderItemResponseDto>? items,
    String? driverName,
  }) {
    return OrderResponseDto(
      id: id ?? this.id,
      customerName: customerName ?? this.customerName,
      customerPhone: customerPhone ?? this.customerPhone,
      customerEmail: customerEmail ?? this.customerEmail,
      deliveryAddress: deliveryAddress ?? this.deliveryAddress,
      subtotal: subtotal ?? this.subtotal,
      deliveryFee: deliveryFee ?? this.deliveryFee,
      discountAmount: discountAmount ?? this.discountAmount,
      couponCode: couponCode ?? this.couponCode,
      total: total ?? this.total,
      status: status ?? this.status,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      isDelivery: isDelivery ?? this.isDelivery,
      notes: notes ?? this.notes,
      estimatedPreparationMinutes: estimatedPreparationMinutes ?? this.estimatedPreparationMinutes,
      estimatedDeliveryTime: estimatedDeliveryTime ?? this.estimatedDeliveryTime,
      queuePosition: queuePosition ?? this.queuePosition,
      orderNumber: orderNumber ?? this.orderNumber,
      items: items ?? this.items,
      driverName: driverName ?? this.driverName,
    );
  }
}
