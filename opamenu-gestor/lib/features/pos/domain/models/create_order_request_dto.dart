import 'package:json_annotation/json_annotation.dart';

part 'create_order_request_dto.g.dart';

@JsonSerializable()
class CreateOrderRequestDto {
  @JsonKey(name: 'customerName')
  final String? customerName;

  @JsonKey(name: 'customerPhone')
  final String? customerPhone;

  @JsonKey(name: 'customerEmail')
  final String? customerEmail;

  @JsonKey(name: 'deliveryAddress')
  final AddressDto? deliveryAddress;

  @JsonKey(name: 'isDelivery')
  final bool isDelivery;

  @JsonKey(name: 'notes')
  final String? notes;

  @JsonKey(name: 'couponCode')
  final String? couponCode;

  @JsonKey(name: 'items')
  final List<CreateOrderItemRequestDto> items;

  @JsonKey(name: 'tableId')
  final String? tableId;

  @JsonKey(name: 'orderType')
  final int orderType;

  @JsonKey(name: 'deliveryFee')
  final double? deliveryFee;

  CreateOrderRequestDto({
    this.customerName,
    this.customerPhone,
    this.customerEmail,
    this.deliveryAddress,
    required this.isDelivery,
    this.notes,
    this.couponCode,
    required this.items,
    this.tableId,
    this.orderType = 0,
    this.deliveryFee,
  });

  factory CreateOrderRequestDto.fromJson(Map<String, dynamic> json) =>
      _$CreateOrderRequestDtoFromJson(json);

  Map<String, dynamic> toJson() => _$CreateOrderRequestDtoToJson(this);
}

@JsonSerializable()
class CreateOrderItemRequestDto {
  @JsonKey(name: 'productId')
  final String productId;

  @JsonKey(name: 'quantity')
  final int quantity;

  @JsonKey(name: 'notes')
  final String? notes;

  @JsonKey(name: 'Aditionals')
  final List<CreateOrderItemAddonRequestDto> addons;

  CreateOrderItemRequestDto({
    required this.productId,
    required this.quantity,
    this.notes,
    this.addons = const [],
  });

  factory CreateOrderItemRequestDto.fromJson(Map<String, dynamic> json) =>
      _$CreateOrderItemRequestDtoFromJson(json);

  Map<String, dynamic> toJson() => _$CreateOrderItemRequestDtoToJson(this);
}

@JsonSerializable()
class CreateOrderItemAddonRequestDto {
  @JsonKey(name: 'AditionalId')
  final String addonId;

  @JsonKey(name: 'quantity')
  final int quantity;

  CreateOrderItemAddonRequestDto({
    required this.addonId,
    required this.quantity,
  });

  factory CreateOrderItemAddonRequestDto.fromJson(Map<String, dynamic> json) =>
      _$CreateOrderItemAddonRequestDtoFromJson(json);

  Map<String, dynamic> toJson() => _$CreateOrderItemAddonRequestDtoToJson(this);
}

@JsonSerializable()
class AddressDto {
  @JsonKey(name: 'zipCode')
  final String zipCode;

  @JsonKey(name: 'street')
  final String street;

  @JsonKey(name: 'number')
  final String number;

  @JsonKey(name: 'complement')
  final String? complement;

  @JsonKey(name: 'neighborhood')
  final String neighborhood;

  @JsonKey(name: 'city')
  final String city;

  @JsonKey(name: 'state')
  final String state;

  AddressDto({
    this.zipCode = '',
    this.street = '',
    this.number = '',
    this.complement,
    this.neighborhood = '',
    this.city = '',
    this.state = '',
  });

  factory AddressDto.fromJson(Map<String, dynamic> json) =>
      _$AddressDtoFromJson(json);

  Map<String, dynamic> toJson() => _$AddressDtoToJson(this);

  @override
  String toString() {
    return '$street, $number${complement != null ? " - $complement" : ""} - $neighborhood, $city - $state, $zipCode';
  }
}
