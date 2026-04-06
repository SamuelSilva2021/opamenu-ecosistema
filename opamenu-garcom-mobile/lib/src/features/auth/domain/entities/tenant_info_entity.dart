class TenantInfoEntity {
  final String id;
  final String name;
  final String slug;
  final String? customDomain;

  const TenantInfoEntity({
    required this.id,
    required this.name,
    required this.slug,
    required this.customDomain,
  });
}

