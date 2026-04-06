import 'roles_basic_entity.dart';

class AccessGroupBasicEntity {
  final String id;
  final String code;
  final List<RolesBasicEntity> roles;

  const AccessGroupBasicEntity({
    required this.id,
    required this.code,
    required this.roles,
  });
}

