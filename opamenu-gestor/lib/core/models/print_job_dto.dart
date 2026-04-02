class PrintItemDto {
  final int qty;
  final String name;
  final double price;
  final String? notes;

  PrintItemDto({
    required this.qty,
    required this.name,
    required this.price,
    this.notes,
  });

  factory PrintItemDto.fromJson(Map<String, dynamic> json) {
    return PrintItemDto(
      qty: json['qty'] as int,
      name: json['name'] as String,
      price: (json['price'] as num).toDouble(),
      notes: json['notes'] as String?,
    );
  }
}

class PrintJobDto {
  final String orderId;
  final String tableNumber;
  final String destination;
  final List<PrintItemDto> items;
  final String? notes;
  final DateTime createdAt;
  final String tenantId;

  PrintJobDto({
    required this.orderId,
    required this.tableNumber,
    required this.destination,
    required this.items,
    this.notes,
    required this.createdAt,
    required this.tenantId,
  });

  factory PrintJobDto.fromJson(Map<String, dynamic> json) {
    var itemsList = json['items'] as List;
    List<PrintItemDto> items = itemsList.map((i) => PrintItemDto.fromJson(i)).toList();

    return PrintJobDto(
      orderId: json['orderId'] as String,
      tableNumber: json['tableNumber'] as String,
      destination: json['destination'] as String,
      items: items,
      notes: json['notes'] as String?,
      createdAt: DateTime.parse(json['createdAt'] as String),
      tenantId: json['tenantId'] as String,
    );
  }
}
