import 'package:json_annotation/json_annotation.dart';

part 'update_table_request_dto.g.dart';

@JsonSerializable()
class UpdateTableRequestDto {
  final String? name;
  final int? capacity;
  final bool? isActive;

  const UpdateTableRequestDto({
    this.name,
    this.capacity,
    this.isActive,
  });

  factory UpdateTableRequestDto.fromJson(Map<String, dynamic> json) =>
      _$UpdateTableRequestDtoFromJson(json);

  Map<String, dynamic> toJson() => _$UpdateTableRequestDtoToJson(this);
}
