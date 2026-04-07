import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../core/data/http/api_http_client_provider.dart';
import '../features/auth/data/datasources/auth_remote_data_source.dart';
import '../features/auth/data/datasources/auth_remote_data_source_contract.dart';
import '../features/auth/data/repositories/auth_repository_impl.dart';
import '../features/auth/domain/repositories/auth_repository.dart';
import '../features/auth/domain/usecases/fetch_current_user_usecase.dart';
import '../features/auth/domain/usecases/login_usecase.dart';
import '../features/auth/domain/usecases/refresh_token_usecase.dart';
import 'app_environment_provider.dart';

class AuthDi {
  static final Provider<AuthRemoteDataSourceContract> remoteDataSourceProvider = Provider(
    (ref) {
      final client = ref.watch(ApiHttpClientProvider.provider);
      final environment = ref.watch(AppEnvironmentProvider.provider);
      return AuthRemoteDataSource(client: client, environment: environment);
    },
  );

  static final Provider<AuthRepository> repositoryProvider = Provider((ref) {
    final remoteDataSource = ref.watch(remoteDataSourceProvider);
    return AuthRepositoryImpl(remoteDataSource);
  });

  static final Provider<LoginUseCase> loginUseCaseProvider = Provider((ref) {
    final repository = ref.watch(repositoryProvider);
    return LoginUseCase(repository);
  });

  static final Provider<FetchCurrentUserUseCase> fetchCurrentUserUseCaseProvider =
      Provider((ref) {
    final repository = ref.watch(repositoryProvider);
    return FetchCurrentUserUseCase(repository);
  });

  static final Provider<RefreshTokenUseCase> refreshTokenUseCaseProvider = Provider((ref) {
    final repository = ref.watch(repositoryProvider);
    return RefreshTokenUseCase(repository);
  });
}
