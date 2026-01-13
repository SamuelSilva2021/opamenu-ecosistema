// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'create_table_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CreateTableRequestDto _$CreateTableRequestDtoFromJson(
  Map<String, dynamic> json,
) => CreateTableRequestDto(
  name: json['name'] as String,
  capacity: (json['capacity'] as num).toInt(),
);

Map<String, dynamic> _$CreateTableRequestDtoToJson(
  CreateTableRequestDto instance,
) => <String, dynamic>{'name': instance.name, 'capacity': instance.capacity};
