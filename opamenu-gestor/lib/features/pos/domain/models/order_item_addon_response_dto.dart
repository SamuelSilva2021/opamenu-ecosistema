import 'package:json_annotation/json_annotation.dart';

part 'order_item_addon_response_dto.g.dart';

@JsonSerializable()
class OrderItemAddonResponseDto {
  final int id;
  final int addonId;
  final String addonName;
  final double unitPrice;
  final int quantity;
  final double subtotal;

  OrderItemAddonResponseDto({
    required this.id,
    required this.addonId,
    required this.addonName,
    required this.unitPrice,
    required this.quantity,
    required this.subtotal,
  });

  factory OrderItemAddonResponseDto.fromJson(Map<String, dynamic> json) =>
      _$OrderItemAddonResponseDtoFromJson(json);

  Map<String, dynamic> toJson() => _$OrderItemAddonResponseDtoToJson(this);
}
