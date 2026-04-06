import '../../../../core/data/json/json_map_reader.dart';
import '../../domain/entities/module_basic_entity.dart';

class ModuleBasicModel {
  final String id;
  final String key;
  final List<String> operations;

  const ModuleBasicModel({
    required this.id,
    required this.key,
    required this.operations,
  });

  factory ModuleBasicModel.fromJson(Map<String, Object?> json) {
    final reader = JsonMapReader(json);
    return ModuleBasicModel(
      id: reader.string('id'),
      key: reader.string('key'),
      operations: reader
          .listOrEmpty('operations')
          .map((e) => e?.toString() ?? '')
          .where((e) => e.isNotEmpty)
          .toList(growable: false),
    );
  }

  ModuleBasicEntity toEntity() {
    return ModuleBasicEntity(id: id, key: key, operations: operations);
  }
}

