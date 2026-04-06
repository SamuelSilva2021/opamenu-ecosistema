import '../../domain/entities/auth_tokens_entity.dart';

class AuthTokensModel {
  final String accessToken;
  final String refreshToken;
  final String tokenType;
  final int expiresIn;
  final String? tenantStatus;
  final int subscriptionStatus;
  final bool requiresPayment;
  final bool redirectToPlanSelection;

  const AuthTokensModel({
    required this.accessToken,
    required this.refreshToken,
    required this.tokenType,
    required this.expiresIn,
    required this.tenantStatus,
    required this.subscriptionStatus,
    required this.requiresPayment,
    required this.redirectToPlanSelection,
  });

  factory AuthTokensModel.fromJson(Map<String, Object?> json) {
    return AuthTokensModel(
      accessToken: _readString(json, 'accessToken', 'AccessToken'),
      refreshToken: _readString(json, 'refreshToken', 'RefreshToken'),
      tokenType: _readString(json, 'tokenType', 'TokenType', defaultValue: 'Bearer'),
      expiresIn: _readInt(json, 'expiresIn', 'ExpiresIn'),
      tenantStatus: _readStringOrNull(json, 'tenantStatus', 'TenantStatus'),
      subscriptionStatus: _readInt(json, 'subscriptionStatus', 'SubscriptionStatus'),
      requiresPayment: _readBool(json, 'requiresPayment', 'RequiresPayment'),
      redirectToPlanSelection: _readBool(
        json,
        'redirectToPlanSelection',
        'RedirectToPlanSelection',
      ),
    );
  }

  AuthTokensEntity toEntity() {
    return AuthTokensEntity(
      accessToken: accessToken,
      refreshToken: refreshToken,
      tokenType: tokenType,
      expiresIn: expiresIn,
      tenantStatus: tenantStatus,
      subscriptionStatus: subscriptionStatus,
      requiresPayment: requiresPayment,
      redirectToPlanSelection: redirectToPlanSelection,
    );
  }

  static String _readString(
    Map<String, Object?> json,
    String camelCase,
    String pascalCase, {
    String defaultValue = '',
  }) {
    final value = json[camelCase] ?? json[pascalCase];
    if (value == null) return defaultValue;
    if (value is String) return value;
    return value.toString();
  }

  static String? _readStringOrNull(
    Map<String, Object?> json,
    String camelCase,
    String pascalCase,
  ) {
    final value = json[camelCase] ?? json[pascalCase];
    if (value == null) return null;
    if (value is String) return value;
    return value.toString();
  }

  static int _readInt(
    Map<String, Object?> json,
    String camelCase,
    String pascalCase,
  ) {
    final value = json[camelCase] ?? json[pascalCase];
    if (value is int) return value;
    if (value is num) return value.toInt();
    if (value is String) return int.tryParse(value) ?? 0;
    return 0;
  }

  static bool _readBool(
    Map<String, Object?> json,
    String camelCase,
    String pascalCase,
  ) {
    final value = json[camelCase] ?? json[pascalCase];
    if (value is bool) return value;
    if (value is num) return value != 0;
    if (value is String) return value.toLowerCase() == 'true';
    return false;
  }
}
