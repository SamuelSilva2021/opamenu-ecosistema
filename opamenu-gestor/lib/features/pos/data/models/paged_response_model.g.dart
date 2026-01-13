// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'paged_response_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PagedResponseModel<T> _$PagedResponseModelFromJson<T>(
  Map<String, dynamic> json,
  T Function(Object? json) fromJsonT,
) => PagedResponseModel<T>(
  succeeded: json['succeeded'] as bool,
  data: _$nullableGenericFromJson(json['data'], fromJsonT),
  errors: (json['errors'] as List<dynamic>?)
      ?.map((e) => ErrorDTO.fromJson(e as Map<String, dynamic>))
      .toList(),
  code: (json['code'] as num?)?.toInt(),
  totalItems: (json['totalItems'] as num).toInt(),
  totalPages: (json['totalPages'] as num).toInt(),
  currentPage: (json['currentPage'] as num).toInt(),
  pageSize: (json['pageSize'] as num).toInt(),
);

T? _$nullableGenericFromJson<T>(
  Object? input,
  T Function(Object? json) fromJson,
) => input == null ? null : fromJson(input);
