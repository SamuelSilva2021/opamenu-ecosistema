// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'table_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TableResponseDto _$TableResponseDtoFromJson(Map<String, dynamic> json) =>
    TableResponseDto(
      id: json['id'] as String,
      name: json['name'] as String,
      capacity: (json['capacity'] as num).toInt(),
      isActive: json['isActive'] as bool,
      qrCodeUrl: json['qrCodeUrl'] as String?,
      layoutX: (json['layoutX'] as num?)?.toDouble(),
      layoutY: (json['layoutY'] as num?)?.toDouble(),
      layoutWidth: (json['layoutWidth'] as num?)?.toDouble(),
      layoutHeight: (json['layoutHeight'] as num?)?.toDouble(),
      floor: json['floor'] as String?,
    );

Map<String, dynamic> _$TableResponseDtoToJson(TableResponseDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'capacity': instance.capacity,
      'isActive': instance.isActive,
      'qrCodeUrl': instance.qrCodeUrl,
      'layoutX': instance.layoutX,
      'layoutY': instance.layoutY,
      'layoutWidth': instance.layoutWidth,
      'layoutHeight': instance.layoutHeight,
      'floor': instance.floor,
    };
