// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'user_info_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

UserInfoModel _$UserInfoModelFromJson(Map<String, dynamic> json) =>
    UserInfoModel(
      id: json['id'] as String,
      username: json['username'] as String,
      email: json['email'] as String,
      fullName: json['fullName'] as String,
      permissions: UserPermissionsModel.fromJson(
        json['permissions'] as Map<String, dynamic>,
      ),
    );

UserPermissionsModel _$UserPermissionsModelFromJson(
  Map<String, dynamic> json,
) => UserPermissionsModel(
  accessGroups: (json['accessGroups'] as List<dynamic>)
      .map((e) => AccessGroupModel.fromJson(e as Map<String, dynamic>))
      .toList(),
);

AccessGroupModel _$AccessGroupModelFromJson(Map<String, dynamic> json) =>
    AccessGroupModel(
      id: json['id'] as String,
      code: json['code'] as String,
      roles: (json['roles'] as List<dynamic>)
          .map((e) => RoleModel.fromJson(e as Map<String, dynamic>))
          .toList(),
    );

RoleModel _$RoleModelFromJson(Map<String, dynamic> json) => RoleModel(
  id: json['id'] as String,
  code: json['code'] as String,
  modules: (json['modules'] as List<dynamic>)
      .map((e) => ModuleModel.fromJson(e as Map<String, dynamic>))
      .toList(),
);

ModuleModel _$ModuleModelFromJson(Map<String, dynamic> json) => ModuleModel(
  id: json['id'] as String,
  key: json['key'] as String,
  operations: (json['operations'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
);
