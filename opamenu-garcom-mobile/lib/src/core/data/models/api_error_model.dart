import '../json/json_map_reader.dart';

class ApiErrorModel {
  final String code;
  final String message;
  final String details;

  const ApiErrorModel({
    required this.code,
    required this.message,
    required this.details,
  });

  factory ApiErrorModel.fromJson(Map<String, Object?> json) {
    final reader = JsonMapReader(json);
    return ApiErrorModel(
      code: reader.string('code'),
      message: reader.string('message'),
      details: reader.stringifyUnknown(json['details']),
    );
  }
}

