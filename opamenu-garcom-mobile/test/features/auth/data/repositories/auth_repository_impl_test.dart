import 'package:flutter_test/flutter_test.dart';
import 'package:opamenu_garcom_mobile/src/core/domain/failures/validation_failure.dart';
import 'package:opamenu_garcom_mobile/src/core/domain/result/failure_result.dart';
import 'package:opamenu_garcom_mobile/src/core/domain/result/success_result.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/data/models/auth_tokens_model.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/data/models/user_info_model.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/data/models/user_permissions_model.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/data/repositories/auth_repository_impl.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/entities/auth_tokens_entity.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/entities/user_info_entity.dart';

import '../../../../support/fake_auth_remote_data_source.dart';

void main() {
  test('mapeia sucesso do login para entity', () async {
    final remote = FakeAuthRemoteDataSource()
      ..nextLoginResult = const SuccessResult(
        AuthTokensModel(
          accessToken: 'a',
          refreshToken: 'r',
          tokenType: 'Bearer',
          expiresIn: 3600,
          tenantStatus: null,
          subscriptionStatus: 0,
          requiresPayment: false,
          redirectToPlanSelection: false,
        ),
      );

    final repository = AuthRepositoryImpl(remote);
    final result = await repository.login(usernameOrEmail: 'u', password: 'p');

    expect(result, isA<SuccessResult<AuthTokensEntity>>());
    expect((result as SuccessResult<AuthTokensEntity>).value.accessToken, 'a');
  });

  test('propaga falha do login', () async {
    final remote = FakeAuthRemoteDataSource()
      ..nextLoginResult = const FailureResult(AuthFailure());

    final repository = AuthRepositoryImpl(remote);
    final result = await repository.login(usernameOrEmail: 'u', password: 'p');

    expect(result, isA<FailureResult<AuthTokensEntity>>());
  });

  test('mapeia sucesso do me para entity', () async {
    final remote = FakeAuthRemoteDataSource()
      ..nextMeResult = SuccessResult(
        UserInfoModel(
          id: 'u1',
          username: 'user',
          email: 'user@email.com',
          fullName: 'User',
          permissions: const UserPermissionsModel(userId: 'u1', accessGroups: []),
          role: null,
          tenant: null,
        ),
      );

    final repository = AuthRepositoryImpl(remote);
    final result = await repository.fetchMe(accessToken: 'token');

    expect(result, isA<SuccessResult<UserInfoEntity>>());
    expect((result as SuccessResult<UserInfoEntity>).value.id, 'u1');
  });

  test('propaga falha do me', () async {
    final remote = FakeAuthRemoteDataSource()..nextMeResult = const FailureResult(AuthFailure());

    final repository = AuthRepositoryImpl(remote);
    final result = await repository.fetchMe(accessToken: 'token');

    expect(result, isA<FailureResult<UserInfoEntity>>());
  });
}

class AuthFailure extends ValidationFailure {
  const AuthFailure() : super('Falha');
}

