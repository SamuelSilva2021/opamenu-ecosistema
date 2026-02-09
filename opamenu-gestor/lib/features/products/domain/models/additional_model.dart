
import 'package:json_annotation/json_annotation.dart';

part 'additional_model.g.dart';

@JsonSerializable()
class AdditionalModel {
  final String id;
  final String name;
  final double price;
  final String? description;
  final bool isActive;
  final String additionalGroupId;

  const AdditionalModel({
    required this.id,
    required this.name,
    required this.price,
    this.description,
    this.isActive = true,
    required this.additionalGroupId,
  });

  factory AdditionalModel.fromJson(Map<String, dynamic> json) => _$AdditionalModelFromJson(json);
  Map<String, dynamic> toJson() => _$AdditionalModelToJson(this);
}
