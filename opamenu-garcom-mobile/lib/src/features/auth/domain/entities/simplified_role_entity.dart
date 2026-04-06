import 'simplified_permission_entity.dart';

class SimplifiedRoleEntity {
  final String id;
  final String name;
  final List<SimplifiedPermissionEntity> permissions;

  const SimplifiedRoleEntity({
    required this.id,
    required this.name,
    required this.permissions,
  });
}

