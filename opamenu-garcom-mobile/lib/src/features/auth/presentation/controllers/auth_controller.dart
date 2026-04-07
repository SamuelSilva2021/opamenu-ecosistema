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
  Future<AuthTokensEntity?>? _refreshInFlight;

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

  Future<AuthTokensEntity?> refreshSession() async {
    final session = state.asData?.value;
    if (session == null) return null;

    final inFlight = _refreshInFlight;
    if (inFlight != null) return inFlight;

    final refreshToken = session.tokens.refreshToken.trim();
    if (refreshToken.isEmpty) {
      signOut();
      return null;
    }

    final future = _refreshTokens(refreshToken: refreshToken, user: session.user);
    _refreshInFlight = future;
    try {
      return await future;
    } finally {
      if (_refreshInFlight == future) _refreshInFlight = null;
    }
  }

  Future<AuthTokensEntity?> _refreshTokens({
    required String refreshToken,
    required UserInfoEntity user,
  }) async {
    final useCase = ref.read(AuthDi.refreshTokenUseCaseProvider);
    final result = await useCase(refreshToken: refreshToken);

    if (result is FailureResult<AuthTokensEntity>) {
      signOut();
      return null;
    }

    final tokens = (result as SuccessResult<AuthTokensEntity>).value;
    state = AsyncData(AuthSessionEntity(tokens: tokens, user: user));
    return tokens;
  }

  void signOut() {
    _activeSignInRequest = null;
    _refreshInFlight = null;
    state = const AsyncData(null);
  }
}
