import '../../../../core/data/json/json_map_reader.dart';
import '../../domain/entities/access_group_basic_entity.dart';
import 'roles_basic_model.dart';

class AccessGroupBasicModel {
  final String id;
  final String code;
  final List<RolesBasicModel> roles;

  const AccessGroupBasicModel({
    required this.id,
    required this.code,
    required this.roles,
  });

  factory AccessGroupBasicModel.fromJson(Map<String, Object?> json) {
    final reader = JsonMapReader(json);
    return AccessGroupBasicModel(
      id: reader.string('id'),
      code: reader.string('code'),
      roles: reader
          .listOrEmpty('roles')
          .whereType<Map>()
          .map((e) => RolesBasicModel.fromJson(e.cast<String, Object?>()))
          .toList(growable: false),
    );
  }

  AccessGroupBasicEntity toEntity() {
    return AccessGroupBasicEntity(
      id: id,
      code: code,
      roles: roles.map((e) => e.toEntity()).toList(growable: false),
    );
  }
}

