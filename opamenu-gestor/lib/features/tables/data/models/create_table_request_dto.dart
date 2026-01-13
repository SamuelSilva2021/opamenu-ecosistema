import 'package:json_annotation/json_annotation.dart';

part 'create_table_request_dto.g.dart';

@JsonSerializable()
class CreateTableRequestDto {
  final String name;
  final int capacity;

  const CreateTableRequestDto({
    required this.name,
    required this.capacity,
  });

  factory CreateTableRequestDto.fromJson(Map<String, dynamic> json) =>
      _$CreateTableRequestDtoFromJson(json);

  Map<String, dynamic> toJson() => _$CreateTableRequestDtoToJson(this);
}
