class TableEntity {
  final String id;
  final String name;
  final bool isActive;
  final int openTabsCount;

  const TableEntity({
    required this.id,
    required this.name,
    required this.isActive,
    required this.openTabsCount,
  });

  bool get isOccupied => openTabsCount > 0;
}
