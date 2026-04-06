class AuthTokensEntity {
  final String accessToken;
  final String refreshToken;
  final String tokenType;
  final int expiresIn;
  final String? tenantStatus;
  final int subscriptionStatus;
  final bool requiresPayment;
  final bool redirectToPlanSelection;

  const AuthTokensEntity({
    required this.accessToken,
    required this.refreshToken,
    required this.tokenType,
    required this.expiresIn,
    required this.tenantStatus,
    required this.subscriptionStatus,
    required this.requiresPayment,
    required this.redirectToPlanSelection,
  });
}

