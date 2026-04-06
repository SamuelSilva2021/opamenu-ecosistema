import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/domain/failures/validation_failure.dart';
import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../domain/entities/auth_session_entity.dart';
import '../../domain/entities/auth_tokens_entity.dart';
import '../../domain/entities/user_info_entity.dart';
import '../../../../app/auth_di.dart';

class AuthController extends AsyncNotifier<AuthSessionEntity?> {
  Object? _activeSignInRequest;

  @override
  Future<AuthSessionEntity?> build() async {
    return null;
  }

  Future<void> signIn({
    required String usernameOrEmail,
    required String password,
  }) async {
    final normalizedUsername = usernameOrEmail.trim();
    if (normalizedUsername.isEmpty) {
      state = AsyncError(
        const ValidationFailure('Informe usuário ou e-mail'),
        StackTrace.current,
      );
      return;
    }

    if (password.isEmpty) {
      state = AsyncError(
        const ValidationFailure('Informe a senha'),
        StackTrace.current,
      );
      return;
    }

    final requestId = Object();
    _activeSignInRequest = requestId;

    state = const AsyncLoading();

    final loginUseCase = ref.read(AuthDi.loginUseCaseProvider);
    final loginResult = await loginUseCase(
      usernameOrEmail: normalizedUsername,
      password: password,
    );

    if (_activeSignInRequest != requestId) return;

    if (loginResult is FailureResult<AuthTokensEntity>) {
      state = AsyncError(loginResult.failure, StackTrace.current);
      return;
    }

    final tokens = (loginResult as SuccessResult<AuthTokensEntity>).value;
    final fetchCurrentUserUseCase = ref.read(AuthDi.fetchCurrentUserUseCaseProvider);
    final meResult = await fetchCurrentUserUseCase(accessToken: tokens.accessToken);

    if (_activeSignInRequest != requestId) return;

    if (meResult is FailureResult<UserInfoEntity>) {
      state = AsyncError(meResult.failure, StackTrace.current);
      return;
    }

    final user = (meResult as SuccessResult<UserInfoEntity>).value;
    state = AsyncData(AuthSessionEntity(tokens: tokens, user: user));
  }

  void signOut() {
    _activeSignInRequest = null;
    state = const AsyncData(null);
  }
}
