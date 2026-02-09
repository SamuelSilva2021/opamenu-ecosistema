
import 'package:json_annotation/json_annotation.dart';

part 'role_model.g.dart';

@JsonSerializable()
class RoleModel {
  final String id;
  final String name;
  final String? description;
  final bool isSystem;

  const RoleModel({
    required this.id,
    required this.name,
    this.description,
    this.isSystem = false,
  });

  factory RoleModel.fromJson(Map<String, dynamic> json) => _$RoleModelFromJson(json);
  Map<String, dynamic> toJson() => _$RoleModelToJson(this);
}
