import '../../../../core/domain/result/result.dart';
import '../entities/user_info_entity.dart';
import '../repositories/auth_repository.dart';

class FetchCurrentUserUseCase {
  final AuthRepository repository;

  const FetchCurrentUserUseCase(this.repository);

  Future<Result<UserInfoEntity>> call({
    required String accessToken,
  }) {
    return repository.fetchMe(accessToken: accessToken);
  }
}

