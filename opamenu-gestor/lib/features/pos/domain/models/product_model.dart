
import 'package:json_annotation/json_annotation.dart';
import 'product_addon_group_response_dto.dart';

part 'product_model.g.dart';

@JsonSerializable()
class ProductModel {
  final String id;
  final String name;
  final double price;
  final String? description;
  final String? imageUrl;
  final String? categoryId;
  final String? categoryName;
  final bool? isActive;
  final int? displayOrder;
  final DateTime? createdAt;
  final DateTime? updatedAt;
  
  @JsonKey(name: 'aditionalGroups', defaultValue: [])
  final List<ProductAddonGroupResponseDto> addonGroups;

  const ProductModel({
    required this.id,
    required this.name,
    required this.price,
    this.description,
    this.imageUrl,
    this.categoryId,
    this.categoryName,
    this.isActive,
    this.displayOrder,
    this.createdAt,
    this.updatedAt,
    this.addonGroups = const [],
  });

  String? get cleanImageUrl {
    if (imageUrl == null) return null;
    // Remove trailing commas and whitespace that might come from API or copy-paste
    // Also removes surrounding quotes or backticks if present
    return imageUrl!
        .replaceAll(',', '')
        .replaceAll('`', '')
        .replaceAll('"', '')
        .replaceAll("'", "")
        .trim();
  }

  factory ProductModel.fromJson(Map<String, dynamic> json) => _$ProductModelFromJson(json);
  Map<String, dynamic> toJson() => _$ProductModelToJson(this);
}
