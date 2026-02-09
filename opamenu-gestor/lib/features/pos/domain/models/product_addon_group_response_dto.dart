
import 'package:json_annotation/json_annotation.dart';
import 'addon_group_response_dto.dart';

part 'product_addon_group_response_dto.g.dart';

@JsonSerializable()
class ProductAddonGroupResponseDto {
  final String id;
  final String productId;
  @JsonKey(name: 'AditionalGroupId')
  final String addonGroupId;
  @JsonKey(name: 'AditionalGroup')
  final AddonGroupResponseDto addonGroup;
  final int displayOrder;
  final bool isRequired;
  final int? minSelectionsOverride;
  final int? maxSelectionsOverride;

  const ProductAddonGroupResponseDto({
    required this.id,
    required this.productId,
    required this.addonGroupId,
    required this.addonGroup,
    required this.displayOrder,
    required this.isRequired,
    this.minSelectionsOverride,
    this.maxSelectionsOverride,
  });

  factory ProductAddonGroupResponseDto.fromJson(Map<String, dynamic> json) =>
      _$ProductAddonGroupResponseDtoFromJson(json);
  Map<String, dynamic> toJson() => _$ProductAddonGroupResponseDtoToJson(this);
}
