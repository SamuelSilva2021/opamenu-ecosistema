// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'order_item_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

OrderItemResponseDto _$OrderItemResponseDtoFromJson(
  Map<String, dynamic> json,
) => OrderItemResponseDto(
  id: json['id'] as String,
  productId: json['productId'] as String,
  productName: json['productName'] as String,
  unitPrice: (json['unitPrice'] as num).toDouble(),
  quantity: (json['quantity'] as num).toInt(),
  subtotal: (json['subtotal'] as num).toDouble(),
  notes: json['notes'] as String?,
  imageUrl: json['imageUrl'] as String?,
  addons:
      (json['addons'] as List<dynamic>?)
          ?.map(
            (e) =>
                OrderItemAddonResponseDto.fromJson(e as Map<String, dynamic>),
          )
          .toList() ??
      const [],
);

Map<String, dynamic> _$OrderItemResponseDtoToJson(
  OrderItemResponseDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'productId': instance.productId,
  'productName': instance.productName,
  'unitPrice': instance.unitPrice,
  'quantity': instance.quantity,
  'subtotal': instance.subtotal,
  'notes': instance.notes,
  'imageUrl': instance.imageUrl,
  'addons': instance.addons,
};
