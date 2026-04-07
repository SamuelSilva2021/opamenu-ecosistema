import '../../../../core/domain/result/result.dart';
import '../entities/auth_tokens_entity.dart';
import '../repositories/auth_repository.dart';

class RefreshTokenUseCase {
  final AuthRepository repository;

  const RefreshTokenUseCase(this.repository);

  Future<Result<AuthTokensEntity>> call({
    required String refreshToken,
  }) {
    return repository.refreshToken(refreshToken: refreshToken);
  }
}

