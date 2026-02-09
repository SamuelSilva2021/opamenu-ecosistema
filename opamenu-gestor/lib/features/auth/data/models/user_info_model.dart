
import 'package:json_annotation/json_annotation.dart';

part 'user_info_model.g.dart';

@JsonSerializable(createToJson: false)
class UserInfoModel {
  final String id;
  final String username;
  final String email;
  final String fullName;
  final UserPermissionsModel permissions;

  UserInfoModel({
    required this.id,
    required this.username,
    required this.email,
    required this.fullName,
    required this.permissions,
  });

  factory UserInfoModel.fromJson(Map<String, dynamic> json) => _$UserInfoModelFromJson(json);
}

@JsonSerializable(createToJson: false)
class UserPermissionsModel {
  @JsonKey(name: 'accessGroups')
  final List<AccessGroupModel> accessGroups;

  UserPermissionsModel({required this.accessGroups});

  factory UserPermissionsModel.fromJson(Map<String, dynamic> json) => _$UserPermissionsModelFromJson(json);
}

@JsonSerializable(createToJson: false)
class AccessGroupModel {
  final String id;
  final String code;
  final List<RoleModel> roles;

  AccessGroupModel({required this.id, required this.code, required this.roles});

  factory AccessGroupModel.fromJson(Map<String, dynamic> json) => _$AccessGroupModelFromJson(json);
}

@JsonSerializable(createToJson: false)
class RoleModel {
  final String id;
  final String code;
  final List<ModuleModel> modules;

  RoleModel({required this.id, required this.code, required this.modules});

  factory RoleModel.fromJson(Map<String, dynamic> json) => _$RoleModelFromJson(json);
}

@JsonSerializable(createToJson: false)
class ModuleModel {
  final String id;
  final String key;
  final List<String> operations;

  ModuleModel({required this.id, required this.key, required this.operations});

  factory ModuleModel.fromJson(Map<String, dynamic> json) => _$ModuleModelFromJson(json);
}
