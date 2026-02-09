// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_order_status_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UpdateOrderStatusRequestDto _$UpdateOrderStatusRequestDtoFromJson(
  Map<String, dynamic> json,
) => UpdateOrderStatusRequestDto(
  status: (json['status'] as num).toInt(),
  notes: json['notes'] as String?,
);

Map<String, dynamic> _$UpdateOrderStatusRequestDtoToJson(
  UpdateOrderStatusRequestDto instance,
) => <String, dynamic>{'status': instance.status, 'notes': instance.notes};
