import 'package:json_annotation/json_annotation.dart';

import '../../../auth/data/models/api_response_model.dart';

part 'paged_response_model.g.dart';

@JsonSerializable(genericArgumentFactories: true, createToJson: false)
class PagedResponseModel<T> {
  final bool succeeded;
  final T? data;
  final List<ErrorDTO>? errors;
  final int? code;
  final int totalItems;
  final int totalPages;
  final int currentPage;
  final int pageSize;

  const PagedResponseModel({
    required this.succeeded,
    this.data,
    this.errors,
    this.code,
    required this.totalItems,
    required this.totalPages,
    required this.currentPage,
    required this.pageSize,
  });

  factory PagedResponseModel.fromJson(
    Map<String, dynamic> json,
    T Function(Object? json) fromJsonT,
  ) =>
      _$PagedResponseModelFromJson(json, fromJsonT);
}