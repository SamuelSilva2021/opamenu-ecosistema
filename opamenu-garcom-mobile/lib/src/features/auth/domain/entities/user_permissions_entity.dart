import 'access_group_basic_entity.dart';

class UserPermissionsEntity {
  final String userId;
  final List<AccessGroupBasicEntity> accessGroups;

  const UserPermissionsEntity({
    required this.userId,
    required this.accessGroups,
  });
}

