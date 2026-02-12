// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'tenant_payment_method_model.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TenantPaymentMethodModel _$TenantPaymentMethodModelFromJson(
  Map<String, dynamic> json,
) => TenantPaymentMethodModel(
  id: (json['id'] as num).toInt(),
  paymentMethodId: (json['paymentMethodId'] as num).toInt(),
  paymentMethod: json['paymentMethod'] == null
      ? null
      : PaymentMethodModel.fromJson(
          json['paymentMethod'] as Map<String, dynamic>,
        ),
  alias: json['alias'] as String?,
  isActive: json['isActive'] as bool,
  configuration: json['configuration'] as String?,
  displayOrder: (json['displayOrder'] as num).toInt(),
);

Map<String, dynamic> _$TenantPaymentMethodModelToJson(
  TenantPaymentMethodModel instance,
) => <String, dynamic>{
  'id': instance.id,
  'paymentMethodId': instance.paymentMethodId,
  'paymentMethod': instance.paymentMethod,
  'alias': instance.alias,
  'isActive': instance.isActive,
  'configuration': instance.configuration,
  'displayOrder': instance.displayOrder,
};

PaymentMethodModel _$PaymentMethodModelFromJson(Map<String, dynamic> json) =>
    PaymentMethodModel(
      id: (json['id'] as num).toInt(),
      name: json['name'] as String,
      slug: json['slug'] as String,
    );

Map<String, dynamic> _$PaymentMethodModelToJson(PaymentMethodModel instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'slug': instance.slug,
    };
