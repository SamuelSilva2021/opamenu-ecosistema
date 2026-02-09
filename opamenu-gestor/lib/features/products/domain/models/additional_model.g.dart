// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'additional_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AdditionalModel _$AdditionalModelFromJson(Map<String, dynamic> json) =>
    AdditionalModel(
      id: json['id'] as String,
      name: json['name'] as String,
      price: (json['price'] as num).toDouble(),
      description: json['description'] as String?,
      isActive: json['isActive'] as bool? ?? true,
      additionalGroupId: json['aditionalGroupId'] as String,
    );

Map<String, dynamic> _$AdditionalModelToJson(AdditionalModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'price': instance.price,
      'description': instance.description,
      'isActive': instance.isActive,
      'aditionalGroupId': instance.additionalGroupId,
    };
