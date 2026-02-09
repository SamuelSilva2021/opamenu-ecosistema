// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'additional_group_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdditionalGroupModel _$AdditionalGroupModelFromJson(
  Map<String, dynamic> json,
) => AdditionalGroupModel(
  id: json['id'] as String,
  name: json['name'] as String,
  description: json['description'] as String?,
  minSelection: (json['minSelection'] as num?)?.toInt() ?? 0,
  maxSelection: (json['maxSelection'] as num?)?.toInt() ?? 1,
  isRequired: json['isRequired'] as bool? ?? false,
  isActive: json['isActive'] as bool? ?? true,
  additionals:
      (json['additionals'] as List<dynamic>?)
          ?.map((e) => AdditionalModel.fromJson(e as Map<String, dynamic>))
          .toList() ??
      const [],
);

Map<String, dynamic> _$AdditionalGroupModelToJson(
  AdditionalGroupModel instance,
) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'description': instance.description,
  'minSelection': instance.minSelection,
  'maxSelection': instance.maxSelection,
  'isRequired': instance.isRequired,
  'isActive': instance.isActive,
  'additionals': instance.additionals,
};
