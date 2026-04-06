import 'package:flutter_test/flutter_test.dart';
import 'package:opamenu_garcom_mobile/src/core/domain/failures/validation_failure.dart';
import 'package:opamenu_garcom_mobile/src/core/domain/result/failure_result.dart';
import 'package:opamenu_garcom_mobile/src/core/domain/result/success_result.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/entities/access_group_basic_entity.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/entities/user_info_entity.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/entities/user_permissions_entity.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/usecases/fetch_current_user_usecase.dart';

import '../../../../support/fake_auth_repository.dart';

void main() {
  test('retorna sucesso quando repository retorna user info', () async {
    final repository = FakeAuthRepository()
      ..nextMeResult = SuccessResult(
        UserInfoEntity(
          id: 'u1',
          username: 'user',
          email: 'user@email.com',
          fullName: 'User',
          permissions: const UserPermissionsEntity(userId: 'u1', accessGroups: <AccessGroupBasicEntity>[]),
          role: null,
          tenant: null,
        ),
      );

    final useCase = FetchCurrentUserUseCase(repository);
    final result = await useCase(accessToken: 'token');

    expect(result, isA<SuccessResult<UserInfoEntity>>());
  });

  test('propaga falha quando repository falha', () async {
    final repository = FakeAuthRepository()..nextMeResult = const FailureResult(AuthFailure());

    final useCase = FetchCurrentUserUseCase(repository);
    final result = await useCase(accessToken: 'token');

    expect(result, isA<FailureResult<UserInfoEntity>>());
  });
}

class AuthFailure extends ValidationFailure {
  const AuthFailure() : super('Token inválido');
}

