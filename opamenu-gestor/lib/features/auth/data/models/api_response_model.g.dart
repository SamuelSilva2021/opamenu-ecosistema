// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'api_response_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ApiResponseModel<T> _$ApiResponseModelFromJson<T>(
  Map<String, dynamic> json,
  T Function(Object? json) fromJsonT,
) => ApiResponseModel<T>(
  succeeded: json['succeeded'] as bool,
  data: _$nullableGenericFromJson(json['data'], fromJsonT),
  errors: (json['errors'] as List<dynamic>?)
      ?.map((e) => ErrorDTO.fromJson(e as Map<String, dynamic>))
      .toList(),
  code: (json['code'] as num?)?.toInt(),
);

T? _$nullableGenericFromJson<T>(
  Object? input,
  T Function(Object? json) fromJson,
) => input == null ? null : fromJson(input);

ErrorDTO _$ErrorDTOFromJson(Map<String, dynamic> json) => ErrorDTO(
  message: json['message'] as String,
  details: (json['details'] as List<dynamic>?)
      ?.map((e) => e as String)
      .toList(),
);
