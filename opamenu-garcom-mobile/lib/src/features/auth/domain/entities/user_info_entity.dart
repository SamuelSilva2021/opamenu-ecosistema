import 'simplified_role_entity.dart';
import 'tenant_info_entity.dart';
import 'user_permissions_entity.dart';

class UserInfoEntity {
  final String id;
  final String username;
  final String email;
  final String fullName;
  final UserPermissionsEntity permissions;
  final SimplifiedRoleEntity? role;
  final TenantInfoEntity? tenant;

  const UserInfoEntity({
    required this.id,
    required this.username,
    required this.email,
    required this.fullName,
    required this.permissions,
    required this.role,
    required this.tenant,
  });
}

