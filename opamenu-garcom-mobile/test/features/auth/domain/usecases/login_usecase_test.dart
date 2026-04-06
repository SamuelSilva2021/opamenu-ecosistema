import 'package:flutter_test/flutter_test.dart';
import 'package:opamenu_garcom_mobile/src/core/domain/failures/validation_failure.dart';
import 'package:opamenu_garcom_mobile/src/core/domain/result/failure_result.dart';
import 'package:opamenu_garcom_mobile/src/core/domain/result/success_result.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/entities/auth_tokens_entity.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/usecases/login_usecase.dart';

import '../../../../support/fake_auth_repository.dart';

void main() {
  test('retorna sucesso quando repository retorna tokens', () async {
    final repository = FakeAuthRepository()
      ..nextLoginResult = const SuccessResult(
        AuthTokensEntity(
          accessToken: 'access',
          refreshToken: 'refresh',
          tokenType: 'Bearer',
          expiresIn: 3600,
          tenantStatus: null,
          subscriptionStatus: 0,
          requiresPayment: false,
          redirectToPlanSelection: false,
        ),
      );

    final useCase = LoginUseCase(repository);
    final result = await useCase(usernameOrEmail: 'u', password: 'p');

    expect(result, isA<SuccessResult<AuthTokensEntity>>());
  });

  test('propaga falha quando repository falha', () async {
    final repository = FakeAuthRepository()
      ..nextLoginResult = const FailureResult(AuthFailure());

    final useCase = LoginUseCase(repository);
    final result = await useCase(usernameOrEmail: 'u', password: 'p');

    expect(result, isA<FailureResult<AuthTokensEntity>>());
  });
}

class AuthFailure extends ValidationFailure {
  const AuthFailure() : super('Credenciais inválidas');
}

