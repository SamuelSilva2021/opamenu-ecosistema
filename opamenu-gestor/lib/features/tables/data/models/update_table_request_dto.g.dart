// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_table_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UpdateTableRequestDto _$UpdateTableRequestDtoFromJson(
  Map<String, dynamic> json,
) => UpdateTableRequestDto(
  name: json['name'] as String?,
  capacity: (json['capacity'] as num?)?.toInt(),
  isActive: json['isActive'] as bool?,
);

Map<String, dynamic> _$UpdateTableRequestDtoToJson(
  UpdateTableRequestDto instance,
) => <String, dynamic>{
  'name': instance.name,
  'capacity': instance.capacity,
  'isActive': instance.isActive,
};
