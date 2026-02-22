// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_movement_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CashMovementResponseDto _$CashMovementResponseDtoFromJson(
  Map<String, dynamic> json,
) => CashMovementResponseDto(
  id: json['id'] as String,
  type: $enumDecode(_$CashMovementTypeEnumMap, json['type']),
  amount: (json['amount'] as num).toDouble(),
  description: json['description'] as String,
  paymentMethod: (json['paymentMethod'] as num?)?.toInt(),
  orderId: json['orderId'] as String?,
  orderNumber: (json['orderNumber'] as num?)?.toInt(),
  createdAt: DateTime.parse(json['createdAt'] as String),
);

Map<String, dynamic> _$CashMovementResponseDtoToJson(
  CashMovementResponseDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'type': _$CashMovementTypeEnumMap[instance.type]!,
  'amount': instance.amount,
  'description': instance.description,
  'paymentMethod': instance.paymentMethod,
  'orderId': instance.orderId,
  'orderNumber': instance.orderNumber,
  'createdAt': instance.createdAt.toIso8601String(),
};

const _$CashMovementTypeEnumMap = {
  CashMovementType.opening: 1,
  CashMovementType.orderPayment: 2,
  CashMovementType.inbound: 3,
  CashMovementType.outbound: 4,
  CashMovementType.reversed: 5,
  CashMovementType.closing: 6,
};
