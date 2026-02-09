
import 'package:json_annotation/json_annotation.dart';

part 'login_response_model.g.dart';

@JsonSerializable(createToJson: false)
class LoginResponseModel {
  final String accessToken;
  final String refreshToken;
  final String tokenType;
  final int expiresIn;
  final String? tenantStatus;
  final String? subscriptionStatus;
  final List<String>? permissions;
  final bool requiresPayment;
  final bool? redirectToPlanSelection;

  const LoginResponseModel({
    required this.accessToken,
    required this.refreshToken,
    this.tokenType = 'Bearer',
    required this.expiresIn,
    this.tenantStatus,
    this.subscriptionStatus,
    this.permissions,
    required this.requiresPayment,
    this.redirectToPlanSelection,
  });

  factory LoginResponseModel.fromJson(Map<String, dynamic> json) =>
      _$LoginResponseModelFromJson(json);
}
