class UpdateTabRequestModel {
  final String? name;
  final String? tableId;

  const UpdateTabRequestModel({
    required this.name,
    required this.tableId,
  });

  Map<String, Object?> toJson() {
    return {
      if (name != null && name!.trim().isNotEmpty) 'name': name!.trim(),
      if (tableId != null && tableId!.trim().isNotEmpty) 'tableId': tableId!.trim(),
    };
  }
}

