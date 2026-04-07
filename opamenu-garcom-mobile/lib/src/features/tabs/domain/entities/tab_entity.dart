class TabEntity {
  final String id;
  final String tableId;
  final String? name;
  final int status;
  final DateTime openedAt;
  final DateTime? closedAt;

  const TabEntity({
    required this.id,
    required this.tableId,
    required this.name,
    required this.status,
    required this.openedAt,
    required this.closedAt,
  });

  bool get isOpen => status == 1;
}

