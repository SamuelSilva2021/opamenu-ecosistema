
import 'package:json_annotation/json_annotation.dart';
import 'additional_model.dart';

part 'additional_group_model.g.dart';

@JsonSerializable()
class AdditionalGroupModel {
  final String id;
  final String name;
  final String? description;
  final int minSelection;
  final int maxSelection;
  final bool isRequired;
  final bool isActive;
  final List<AdditionalModel> additionals;

  const AdditionalGroupModel({
    required this.id,
    required this.name,
    this.description,
    this.minSelection = 0,
    this.maxSelection = 1,
    this.isRequired = false,
    this.isActive = true,
    this.additionals = const [],
  });

  factory AdditionalGroupModel.fromJson(Map<String, dynamic> json) => _$AdditionalGroupModelFromJson(json);
  Map<String, dynamic> toJson() => _$AdditionalGroupModelToJson(this);
}
