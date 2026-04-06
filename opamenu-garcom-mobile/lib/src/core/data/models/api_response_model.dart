import '../json/json_map_reader.dart';
import 'api_error_model.dart';

class ApiResponseModel<T> {
  final bool succeeded;
  final Object? successResult;
  final List<ApiErrorModel> errors;
  final T? data;

  const ApiResponseModel({
    required this.succeeded,
    required this.successResult,
    required this.errors,
    required this.data,
  });

  factory ApiResponseModel.fromJson(
    Map<String, Object?> json, {
    required T Function(Map<String, Object?> json) dataParser,
  }) {
    final reader = JsonMapReader(json);
    final dataMap = reader.mapOrNull('data');
    return ApiResponseModel(
      succeeded: reader.boolValue('succeeded'),
      successResult: json['successResult'],
      errors: reader
          .listOrEmpty('errors')
          .whereType<Map>()
          .map((e) => ApiErrorModel.fromJson(e.cast<String, Object?>()))
          .toList(growable: false),
      data: dataMap == null ? null : dataParser(dataMap),
    );
  }
}

