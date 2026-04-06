import '../../../../core/domain/result/result.dart';
import '../entities/auth_tokens_entity.dart';
import '../repositories/auth_repository.dart';

class LoginUseCase {
  final AuthRepository repository;

  const LoginUseCase(this.repository);

  Future<Result<AuthTokensEntity>> call({
    required String usernameOrEmail,
    required String password,
  }) {
    return repository.login(
      usernameOrEmail: usernameOrEmail,
      password: password,
    );
  }
}

