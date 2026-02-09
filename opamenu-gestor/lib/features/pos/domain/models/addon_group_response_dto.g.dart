// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'addon_group_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AddonGroupResponseDto _$AddonGroupResponseDtoFromJson(
  Map<String, dynamic> json,
) => AddonGroupResponseDto(
  id: json['id'] as String,
  name: json['name'] as String,
  description: json['description'] as String?,
  minSelections: (json['minSelections'] as num).toInt(),
  maxSelections: (json['maxSelections'] as num).toInt(),
  isRequired: json['isRequired'] as bool,
  isActive: json['isActive'] as bool,
  addons: json['addons'] as List<dynamic>?,
);

Map<String, dynamic> _$AddonGroupResponseDtoToJson(
  AddonGroupResponseDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'description': instance.description,
  'minSelections': instance.minSelections,
  'maxSelections': instance.maxSelections,
  'isRequired': instance.isRequired,
  'isActive': instance.isActive,
  'addons': instance.addons,
};
