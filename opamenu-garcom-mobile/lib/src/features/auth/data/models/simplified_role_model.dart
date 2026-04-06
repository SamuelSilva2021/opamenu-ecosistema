import '../../../../core/data/json/json_map_reader.dart';
import '../../domain/entities/simplified_role_entity.dart';
import 'simplified_permission_model.dart';

class SimplifiedRoleModel {
  final String id;
  final String name;
  final List<SimplifiedPermissionModel> permissions;

  const SimplifiedRoleModel({
    required this.id,
    required this.name,
    required this.permissions,
  });

  factory SimplifiedRoleModel.fromJson(Map<String, Object?> json) {
    final reader = JsonMapReader(json);
    return SimplifiedRoleModel(
      id: reader.string('id'),
      name: reader.string('name'),
      permissions: reader
          .listOrEmpty('permissions')
          .whereType<Map>()
          .map((e) => SimplifiedPermissionModel.fromJson(e.cast<String, Object?>()))
          .toList(growable: false),
    );
  }

  SimplifiedRoleEntity toEntity() {
    return SimplifiedRoleEntity(
      id: id,
      name: name,
      permissions: permissions.map((e) => e.toEntity()).toList(growable: false),
    );
  }
}

