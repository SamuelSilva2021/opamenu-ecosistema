import 'package:json_annotation/json_annotation.dart';

part 'collaborator_model.g.dart';

@JsonSerializable()
class CollaboratorModel {
  final String id;
  final String name;
  final String? phone;
  final int type; // 1: Internal, 2: External
  final String? role;
  final bool active;
  final String? userAccountId;
  final String tenantId;
  final DateTime createdAt;

  CollaboratorModel({
    required this.id,
    required this.name,
    this.phone,
    required this.type,
    this.role,
    required this.active,
    this.userAccountId,
    required this.tenantId,
    required this.createdAt,
  });

  factory CollaboratorModel.fromJson(Map<String, dynamic> json) =>
      _$CollaboratorModelFromJson(json);

  Map<String, dynamic> toJson() => _$CollaboratorModelToJson(this);

  String get typeLabel => type == 1 ? 'Interno' : 'Externo';
}
