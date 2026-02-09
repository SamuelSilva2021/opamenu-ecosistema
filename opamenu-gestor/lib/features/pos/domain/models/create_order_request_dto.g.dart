// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_order_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreateOrderRequestDto _$CreateOrderRequestDtoFromJson(
  Map<String, dynamic> json,
) => CreateOrderRequestDto(
  customerName: json['customerName'] as String?,
  customerPhone: json['customerPhone'] as String?,
  customerEmail: json['customerEmail'] as String?,
  deliveryAddress: json['deliveryAddress'] == null
      ? null
      : AddressDto.fromJson(json['deliveryAddress'] as Map<String, dynamic>),
  isDelivery: json['isDelivery'] as bool,
  notes: json['notes'] as String?,
  couponCode: json['couponCode'] as String?,
  items: (json['items'] as List<dynamic>)
      .map((e) => CreateOrderItemRequestDto.fromJson(e as Map<String, dynamic>))
      .toList(),
  tableId: json['tableId'] as String?,
  orderType: (json['orderType'] as num?)?.toInt() ?? 0,
);

Map<String, dynamic> _$CreateOrderRequestDtoToJson(
  CreateOrderRequestDto instance,
) => <String, dynamic>{
  'customerName': instance.customerName,
  'customerPhone': instance.customerPhone,
  'customerEmail': instance.customerEmail,
  'deliveryAddress': instance.deliveryAddress,
  'isDelivery': instance.isDelivery,
  'notes': instance.notes,
  'couponCode': instance.couponCode,
  'items': instance.items,
  'tableId': instance.tableId,
  'orderType': instance.orderType,
};

CreateOrderItemRequestDto _$CreateOrderItemRequestDtoFromJson(
  Map<String, dynamic> json,
) => CreateOrderItemRequestDto(
  productId: json['productId'] as String,
  quantity: (json['quantity'] as num).toInt(),
  notes: json['notes'] as String?,
  addons:
      (json['Aditionals'] as List<dynamic>?)
          ?.map(
            (e) => CreateOrderItemAddonRequestDto.fromJson(
              e as Map<String, dynamic>,
            ),
          )
          .toList() ??
      const [],
);

Map<String, dynamic> _$CreateOrderItemRequestDtoToJson(
  CreateOrderItemRequestDto instance,
) => <String, dynamic>{
  'productId': instance.productId,
  'quantity': instance.quantity,
  'notes': instance.notes,
  'Aditionals': instance.addons,
};

CreateOrderItemAddonRequestDto _$CreateOrderItemAddonRequestDtoFromJson(
  Map<String, dynamic> json,
) => CreateOrderItemAddonRequestDto(
  addonId: json['AditionalId'] as String,
  quantity: (json['quantity'] as num).toInt(),
);

Map<String, dynamic> _$CreateOrderItemAddonRequestDtoToJson(
  CreateOrderItemAddonRequestDto instance,
) => <String, dynamic>{
  'AditionalId': instance.addonId,
  'quantity': instance.quantity,
};

AddressDto _$AddressDtoFromJson(Map<String, dynamic> json) => AddressDto(
  zipCode: json['zipCode'] as String? ?? '',
  street: json['street'] as String? ?? '',
  number: json['number'] as String? ?? '',
  complement: json['complement'] as String?,
  neighborhood: json['neighborhood'] as String? ?? '',
  city: json['city'] as String? ?? '',
  state: json['state'] as String? ?? '',
);

Map<String, dynamic> _$AddressDtoToJson(AddressDto instance) =>
    <String, dynamic>{
      'zipCode': instance.zipCode,
      'street': instance.street,
      'number': instance.number,
      'complement': instance.complement,
      'neighborhood': instance.neighborhood,
      'city': instance.city,
      'state': instance.state,
    };
