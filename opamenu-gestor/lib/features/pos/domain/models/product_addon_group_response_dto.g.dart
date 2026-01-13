// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'product_addon_group_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ProductAddonGroupResponseDto _$ProductAddonGroupResponseDtoFromJson(
  Map<String, dynamic> json,
) => ProductAddonGroupResponseDto(
  id: (json['id'] as num).toInt(),
  productId: (json['productId'] as num).toInt(),
  addonGroupId: (json['addonGroupId'] as num).toInt(),
  addonGroup: AddonGroupResponseDto.fromJson(
    json['addonGroup'] as Map<String, dynamic>,
  ),
  displayOrder: (json['displayOrder'] as num).toInt(),
  isRequired: json['isRequired'] as bool,
  minSelectionsOverride: (json['minSelectionsOverride'] as num?)?.toInt(),
  maxSelectionsOverride: (json['maxSelectionsOverride'] as num?)?.toInt(),
);

Map<String, dynamic> _$ProductAddonGroupResponseDtoToJson(
  ProductAddonGroupResponseDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'productId': instance.productId,
  'addonGroupId': instance.addonGroupId,
  'addonGroup': instance.addonGroup,
  'displayOrder': instance.displayOrder,
  'isRequired': instance.isRequired,
  'minSelectionsOverride': instance.minSelectionsOverride,
  'maxSelectionsOverride': instance.maxSelectionsOverride,
};
