// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'order_item_addon_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

OrderItemAddonResponseDto _$OrderItemAddonResponseDtoFromJson(
  Map<String, dynamic> json,
) => OrderItemAddonResponseDto(
  id: (json['id'] as num).toInt(),
  addonId: (json['addonId'] as num).toInt(),
  addonName: json['addonName'] as String,
  unitPrice: (json['unitPrice'] as num).toDouble(),
  quantity: (json['quantity'] as num).toInt(),
  subtotal: (json['subtotal'] as num).toDouble(),
);

Map<String, dynamic> _$OrderItemAddonResponseDtoToJson(
  OrderItemAddonResponseDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'addonId': instance.addonId,
  'addonName': instance.addonName,
  'unitPrice': instance.unitPrice,
  'quantity': instance.quantity,
  'subtotal': instance.subtotal,
};
