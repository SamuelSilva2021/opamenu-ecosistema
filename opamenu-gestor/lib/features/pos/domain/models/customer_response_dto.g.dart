// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'customer_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CustomerResponseDto _$CustomerResponseDtoFromJson(Map<String, dynamic> json) =>
    CustomerResponseDto(
      id: json['id'] as String,
      name: json['name'] as String,
      phone: json['phone'] as String,
      email: json['email'] as String?,
      postalCode: json['postalCode'] as String?,
      street: json['street'] as String?,
      streetNumber: json['streetNumber'] as String?,
      neighborhood: json['neighborhood'] as String?,
      city: json['city'] as String?,
      state: json['state'] as String?,
      complement: json['complement'] as String?,
    );

Map<String, dynamic> _$CustomerResponseDtoToJson(
  CustomerResponseDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'phone': instance.phone,
  'email': instance.email,
  'postalCode': instance.postalCode,
  'street': instance.street,
  'streetNumber': instance.streetNumber,
  'neighborhood': instance.neighborhood,
  'city': instance.city,
  'state': instance.state,
  'complement': instance.complement,
};
