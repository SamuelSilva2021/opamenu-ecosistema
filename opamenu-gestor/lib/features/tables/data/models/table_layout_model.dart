class TableLayoutModel {
  final String tableId;
  final double x;
  final double y;
  final double width;
  final double height;
  final String? floor;

  const TableLayoutModel({
    required this.tableId,
    required this.x,
    required this.y,
    this.width = 100,
    this.height = 100,
    this.floor,
  });

  Map<String, dynamic> toJson() => {
        'tableId': tableId,
        'x': x,
        'y': y,
        'width': width,
        'height': height,
        if (floor != null) 'floor': floor,
      };

  factory TableLayoutModel.fromJson(Map<String, dynamic> json) =>
      TableLayoutModel(
        tableId: json['tableId'] as String,
        x: (json['x'] as num).toDouble(),
        y: (json['y'] as num).toDouble(),
        width: (json['width'] as num?)?.toDouble() ?? 100.0,
        height: (json['height'] as num?)?.toDouble() ?? 100.0,
        floor: json['floor']?.toString(),
      );
}
