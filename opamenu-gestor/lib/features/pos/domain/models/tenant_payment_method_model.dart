import 'package:json_annotation/json_annotation.dart';

part 'tenant_payment_method_model.g.dart';

@JsonSerializable()
class TenantPaymentMethodModel {
  final int id;
  final int paymentMethodId;
  final PaymentMethodModel? paymentMethod;
  final String? alias;
  final bool isActive;
  final String? configuration;
  final int displayOrder;

  TenantPaymentMethodModel({
    required this.id,
    required this.paymentMethodId,
    this.paymentMethod,
    this.alias,
    required this.isActive,
    this.configuration,
    required this.displayOrder,
  });

  factory TenantPaymentMethodModel.fromJson(Map<String, dynamic> json) => _$TenantPaymentMethodModelFromJson(json);
  Map<String, dynamic> toJson() => _$TenantPaymentMethodModelToJson(this);
}

@JsonSerializable()
class PaymentMethodModel {
  final int id;
  final String name;
  final String slug;
  
  PaymentMethodModel({required this.id, required this.name, required this.slug});
  
  factory PaymentMethodModel.fromJson(Map<String, dynamic> json) => _$PaymentMethodModelFromJson(json);
  Map<String, dynamic> toJson() => _$PaymentMethodModelToJson(this);
}
