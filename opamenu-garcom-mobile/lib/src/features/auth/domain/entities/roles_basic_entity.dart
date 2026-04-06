import 'module_basic_entity.dart';

class RolesBasicEntity {
  final String id;
  final String code;
  final List<ModuleBasicEntity> modules;

  const RolesBasicEntity({
    required this.id,
    required this.code,
    required this.modules,
  });
}

