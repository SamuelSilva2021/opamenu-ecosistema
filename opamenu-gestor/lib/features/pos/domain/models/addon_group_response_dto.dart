
import 'package:json_annotation/json_annotation.dart';

part 'addon_group_response_dto.g.dart';

@JsonSerializable()
class AddonGroupResponseDto {
  final String id;
  final String name;
  final String? description;
  final int minSelections;
  final int maxSelections;
  final bool isRequired;
  final bool isActive;
  final List<dynamic>? addons; // Pode ser tipado melhor futuramente se tivermos o DTO de addons

  const AddonGroupResponseDto({
    required this.id,
    required this.name,
    this.description,
    required this.minSelections,
    required this.maxSelections,
    required this.isRequired,
    required this.isActive,
    this.addons,
  });

  factory AddonGroupResponseDto.fromJson(Map<String, dynamic> json) =>
      _$AddonGroupResponseDtoFromJson(json);
  Map<String, dynamic> toJson() => _$AddonGroupResponseDtoToJson(this);
}
