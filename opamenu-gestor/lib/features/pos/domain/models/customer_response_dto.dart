import 'package:json_annotation/json_annotation.dart';

part 'customer_response_dto.g.dart';

@JsonSerializable()
class CustomerResponseDto {
  final String id;
  final String name;
  final String phone;
  final String? email;
  final String? postalCode;
  final String? street;
  final String? streetNumber;
  final String? neighborhood;
  final String? city;
  final String? state;
  final String? complement;

  CustomerResponseDto({
    required this.id,
    required this.name,
    required this.phone,
    this.email,
    this.postalCode,
    this.street,
    this.streetNumber,
    this.neighborhood,
    this.city,
    this.state,
    this.complement,
  });

  factory CustomerResponseDto.fromJson(Map<String, dynamic> json) =>
      _$CustomerResponseDtoFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerResponseDtoToJson(this);
}
