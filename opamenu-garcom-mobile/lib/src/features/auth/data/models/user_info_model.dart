import '../../../../core/data/json/json_map_reader.dart';
import '../../domain/entities/user_info_entity.dart';
import 'simplified_role_model.dart';
import 'tenant_info_model.dart';
import 'user_permissions_model.dart';

class UserInfoModel {
  final String id;
  final String username;
  final String email;
  final String fullName;
  final UserPermissionsModel permissions;
  final SimplifiedRoleModel? role;
  final TenantInfoModel? tenant;

  const UserInfoModel({
    required this.id,
    required this.username,
    required this.email,
    required this.fullName,
    required this.permissions,
    required this.role,
    required this.tenant,
  });

  factory UserInfoModel.fromJson(Map<String, Object?> json) {
    final reader = JsonMapReader(json);
    final permissionsMap = reader.mapOrNull('permissions') ?? const <String, Object?>{};
    final roleMap = reader.mapOrNull('role');
    final tenantMap = reader.mapOrNull('tenant');
    return UserInfoModel(
      id: reader.string('id'),
      username: reader.string('username'),
      email: reader.string('email'),
      fullName: reader.string('fullName'),
      permissions: UserPermissionsModel.fromJson(permissionsMap),
      role: roleMap == null ? null : SimplifiedRoleModel.fromJson(roleMap),
      tenant: tenantMap == null ? null : TenantInfoModel.fromJson(tenantMap),
    );
  }

  UserInfoEntity toEntity() {
    return UserInfoEntity(
      id: id,
      username: username,
      email: email,
      fullName: fullName,
      permissions: permissions.toEntity(),
      role: role?.toEntity(),
      tenant: tenant?.toEntity(),
    );
  }
}

