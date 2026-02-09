
import 'package:json_annotation/json_annotation.dart';

part 'permission_model.g.dart';

@JsonSerializable()
class PermissionModel {
  final String module;
  final String? operation;

  PermissionModel({
    required this.module,
    this.operation,
  });

  factory PermissionModel.fromString(String permission) {
    final parts = permission.split(':');
    return PermissionModel(
      module: parts[0],
      operation: parts.length > 1 ? parts[1] : null,
    );
  }

  String get key => operation != null ? '$module:$operation' : module;

  factory PermissionModel.fromJson(Map<String, dynamic> json) =>
      _$PermissionModelFromJson(json);

  Map<String, dynamic> toJson() => _$PermissionModelToJson(this);

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is PermissionModel &&
          runtimeType == other.runtimeType &&
          module == other.module &&
          operation == other.operation;

  @override
  int get hashCode => module.hashCode ^ operation.hashCode;
}
