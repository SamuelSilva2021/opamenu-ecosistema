import '../../../../core/data/json/json_map_reader.dart';
import '../../domain/entities/simplified_permission_entity.dart';

class SimplifiedPermissionModel {
  final String module;
  final List<String> actions;

  const SimplifiedPermissionModel({
    required this.module,
    required this.actions,
  });

  factory SimplifiedPermissionModel.fromJson(Map<String, Object?> json) {
    final reader = JsonMapReader(json);
    return SimplifiedPermissionModel(
      module: reader.string('module'),
      actions: reader
          .listOrEmpty('actions')
          .map((e) => e?.toString() ?? '')
          .where((e) => e.isNotEmpty)
          .toList(growable: false),
    );
  }

  SimplifiedPermissionEntity toEntity() {
    return SimplifiedPermissionEntity(module: module, actions: actions);
  }
}

