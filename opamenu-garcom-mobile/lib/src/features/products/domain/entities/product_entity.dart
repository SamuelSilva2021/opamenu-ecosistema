class ProductEntity {
  final String id;
  final String name;
  final double price;
  final String categoryName;
  final bool isActive;
  final String? imageUrl;

  const ProductEntity({
    required this.id,
    required this.name,
    required this.price,
    required this.categoryName,
    required this.isActive,
    required this.imageUrl,
  });
}

