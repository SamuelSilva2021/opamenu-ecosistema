import 'package:json_annotation/json_annotation.dart';
import 'order_item_addon_response_dto.dart';

part 'order_item_response_dto.g.dart';

@JsonSerializable()
class OrderItemResponseDto {
  final String id;
  final String productId;
  final String productName;
  final double unitPrice;
  final int quantity;
  final double subtotal;
  final String? notes;
  final String? imageUrl;
  final int status;
  final List<OrderItemAddonResponseDto> addons;

  OrderItemResponseDto({
    required this.id,
    required this.productId,
    required this.productName,
    required this.unitPrice,
    required this.quantity,
    required this.subtotal,
    this.notes,
    this.imageUrl,
    this.status = 0,
    this.addons = const [],
  });

  factory OrderItemResponseDto.fromJson(Map<String, dynamic> json) =>
      _$OrderItemResponseDtoFromJson(json);

  Map<String, dynamic> toJson() => _$OrderItemResponseDtoToJson(this);

  OrderItemResponseDto copyWith({
    String? id,
    String? productId,
    String? productName,
    double? unitPrice,
    int? quantity,
    double? subtotal,
    String? notes,
    String? imageUrl,
    int? status,
    List<OrderItemAddonResponseDto>? addons,
  }) {
    return OrderItemResponseDto(
      id: id ?? this.id,
      productId: productId ?? this.productId,
      productName: productName ?? this.productName,
      unitPrice: unitPrice ?? this.unitPrice,
      quantity: quantity ?? this.quantity,
      subtotal: subtotal ?? this.subtotal,
      notes: notes ?? this.notes,
      imageUrl: imageUrl ?? this.imageUrl,
      status: status ?? this.status,
      addons: addons ?? this.addons,
    );
  }
}
