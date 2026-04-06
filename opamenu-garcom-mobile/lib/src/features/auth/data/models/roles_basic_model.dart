import '../../../../core/data/json/json_map_reader.dart';
import '../../domain/entities/roles_basic_entity.dart';
import 'module_basic_model.dart';

class RolesBasicModel {
  final String id;
  final String code;
  final List<ModuleBasicModel> modules;

  const RolesBasicModel({
    required this.id,
    required this.code,
    required this.modules,
  });

  factory RolesBasicModel.fromJson(Map<String, Object?> json) {
    final reader = JsonMapReader(json);
    return RolesBasicModel(
      id: reader.string('id'),
      code: reader.string('code'),
      modules: reader
          .listOrEmpty('modules')
          .whereType<Map>()
          .map((e) => ModuleBasicModel.fromJson(e.cast<String, Object?>()))
          .toList(growable: false),
    );
  }

  RolesBasicEntity toEntity() {
    return RolesBasicEntity(
      id: id,
      code: code,
      modules: modules.map((e) => e.toEntity()).toList(growable: false),
    );
  }
}

