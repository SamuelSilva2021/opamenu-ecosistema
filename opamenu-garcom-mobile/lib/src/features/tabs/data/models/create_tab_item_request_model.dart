class CreateTabItemRequestModel {
  final String productId;
  final int quantity;
  final String? notes;

  const CreateTabItemRequestModel({
    required this.productId,
    required this.quantity,
    required this.notes,
  });

  Map<String, Object?> toJson() {
    return {
      'productId': productId,
      'quantity': quantity,
      if (notes != null && notes!.trim().isNotEmpty) 'notes': notes!.trim(),
      'aditionals': const <Object?>[],
    };
  }
}

