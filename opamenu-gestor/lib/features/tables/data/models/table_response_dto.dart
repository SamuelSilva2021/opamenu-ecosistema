import 'package:json_annotation/json_annotation.dart';

part 'table_response_dto.g.dart';

@JsonSerializable()
class TableResponseDto {
  final String id;
  final String name;
  final int capacity;
  final bool isActive;
  final String? qrCodeUrl;

  const TableResponseDto({
    required this.id,
    required this.name,
    required this.capacity,
    required this.isActive,
    this.qrCodeUrl,
  });

  factory TableResponseDto.fromJson(Map<String, dynamic> json) =>
      _$TableResponseDtoFromJson(json);

  Map<String, dynamic> toJson() => _$TableResponseDtoToJson(this);
}
