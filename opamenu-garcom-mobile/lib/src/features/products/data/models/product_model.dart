import '../../domain/entities/product_entity.dart';

class ProductModel {
  final String id;
  final String name;
  final double price;
  final String categoryName;
  final bool isActive;
  final String? imageUrl;

  const ProductModel({
    required this.id,
    required this.name,
    required this.price,
    required this.categoryName,
    required this.isActive,
    required this.imageUrl,
  });

  factory ProductModel.fromJson(Map<String, Object?> json) {
    final rawPrice = json['price'];
    final parsedPrice = rawPrice is num ? rawPrice.toDouble() : double.tryParse(rawPrice?.toString() ?? '');

    return ProductModel(
      id: json['id']?.toString() ?? '',
      name: json['name']?.toString() ?? '',
      price: parsedPrice ?? 0,
      categoryName: json['categoryName']?.toString() ?? '',
      isActive: json['isActive'] is bool ? json['isActive'] as bool : true,
      imageUrl: json['imageUrl']?.toString(),
    );
  }

  ProductEntity toEntity() {
    return ProductEntity(
      id: id,
      name: name,
      price: price,
      categoryName: categoryName,
      isActive: isActive,
      imageUrl: imageUrl,
    );
  }
}

