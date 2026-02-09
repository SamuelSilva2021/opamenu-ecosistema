
import 'package:json_annotation/json_annotation.dart';
import 'role_model.dart';

part 'user_model.g.dart';

@JsonSerializable()
class UserModel {
  final String id;
  final String username;
  final String email;
  final String? phoneNumber;
  final bool isActive;
  final List<RoleModel> roles;

  const UserModel({
    required this.id,
    required this.username,
    required this.email,
    this.phoneNumber,
    required this.isActive,
    this.roles = const [],
  });

  factory UserModel.fromJson(Map<String, dynamic> json) => _$UserModelFromJson(json);
  Map<String, dynamic> toJson() => _$UserModelToJson(this);
}
