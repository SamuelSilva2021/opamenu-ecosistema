import '../../../../core/data/json/json_map_reader.dart';
import '../../domain/entities/tenant_info_entity.dart';

class TenantInfoModel {
  final String id;
  final String name;
  final String slug;
  final String? customDomain;

  const TenantInfoModel({
    required this.id,
    required this.name,
    required this.slug,
    required this.customDomain,
  });

  factory TenantInfoModel.fromJson(Map<String, Object?> json) {
    final reader = JsonMapReader(json);
    return TenantInfoModel(
      id: reader.string('id'),
      name: reader.string('name'),
      slug: reader.string('slug'),
      customDomain: reader.stringOrNull('customDomain'),
    );
  }

  TenantInfoEntity toEntity() {
    return TenantInfoEntity(
      id: id,
      name: name,
      slug: slug,
      customDomain: customDomain,
    );
  }
}

