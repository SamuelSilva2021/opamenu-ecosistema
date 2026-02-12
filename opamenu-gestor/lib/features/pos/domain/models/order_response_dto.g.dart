// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'order_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

OrderResponseDto _$OrderResponseDtoFromJson(Map<String, dynamic> json) =>
    OrderResponseDto(
      id: json['id'] as String,
      customerName: json['customerName'] as String,
      customerPhone: json['customerPhone'] as String,
      customerEmail: json['customerEmail'] as String?,
      deliveryAddress: json['deliveryAddress'] as String?,
      subtotal: (json['subtotal'] as num).toDouble(),
      deliveryFee: (json['deliveryFee'] as num).toDouble(),
      discountAmount: (json['discountAmount'] as num).toDouble(),
      couponCode: json['couponCode'] as String?,
      total: (json['total'] as num).toDouble(),
      status: $enumDecode(_$OrderStatusEnumMap, json['status']),
      createdAt: DateTime.parse(json['createdAt'] as String),
      updatedAt: json['updatedAt'] == null
          ? null
          : DateTime.parse(json['updatedAt'] as String),
      isDelivery: json['isDelivery'] as bool,
      notes: json['notes'] as String?,
      estimatedPreparationMinutes: (json['estimatedPreparationMinutes'] as num?)
          ?.toInt(),
      estimatedDeliveryTime: json['estimatedDeliveryTime'] == null
          ? null
          : DateTime.parse(json['estimatedDeliveryTime'] as String),
      queuePosition: (json['queuePosition'] as num).toInt(),
      orderNumber: (json['orderNumber'] as num?)?.toInt(),
      items:
          (json['items'] as List<dynamic>?)
              ?.map(
                (e) => OrderItemResponseDto.fromJson(e as Map<String, dynamic>),
              )
              .toList() ??
          const [],
      driverName: json['driverName'] as String?,
    );

Map<String, dynamic> _$OrderResponseDtoToJson(
  OrderResponseDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'customerName': instance.customerName,
  'customerPhone': instance.customerPhone,
  'customerEmail': instance.customerEmail,
  'deliveryAddress': instance.deliveryAddress,
  'subtotal': instance.subtotal,
  'deliveryFee': instance.deliveryFee,
  'discountAmount': instance.discountAmount,
  'couponCode': instance.couponCode,
  'total': instance.total,
  'status': _$OrderStatusEnumMap[instance.status]!,
  'createdAt': instance.createdAt.toIso8601String(),
  'updatedAt': instance.updatedAt?.toIso8601String(),
  'isDelivery': instance.isDelivery,
  'notes': instance.notes,
  'estimatedPreparationMinutes': instance.estimatedPreparationMinutes,
  'estimatedDeliveryTime': instance.estimatedDeliveryTime?.toIso8601String(),
  'queuePosition': instance.queuePosition,
  'orderNumber': instance.orderNumber,
  'items': instance.items,
  'driverName': instance.driverName,
};

const _$OrderStatusEnumMap = {
  OrderStatus.pending: 0,
  OrderStatus.preparing: 1,
  OrderStatus.ready: 2,
  OrderStatus.outForDelivery: 3,
  OrderStatus.delivered: 4,
  OrderStatus.cancelled: 5,
  OrderStatus.rejected: 6,
};
