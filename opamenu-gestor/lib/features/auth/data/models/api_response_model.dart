
import 'package:json_annotation/json_annotation.dart';

part 'api_response_model.g.dart';

@JsonSerializable(genericArgumentFactories: true, createToJson: false)
class ApiResponseModel<T> {
  final bool succeeded;
  final T? data;
  final List<ErrorDTO>? errors;
  final int? code;

  const ApiResponseModel({
    required this.succeeded,
    this.data,
    this.errors,
    this.code,
  });

  factory ApiResponseModel.fromJson(
    Map<String, dynamic> json,
    T Function(Object? json) fromJsonT,
  ) =>
      _$ApiResponseModelFromJson(json, fromJsonT);
}

@JsonSerializable(createToJson: false)
class ErrorDTO {
  final String message;
  final List<String>? details;

  const ErrorDTO({
    required this.message,
    this.details,
  });

  factory ErrorDTO.fromJson(Map<String, dynamic> json) =>
      _$ErrorDTOFromJson(json);
}
