import '../../../../core/domain/result/result.dart';
import '../entities/auth_tokens_entity.dart';
import '../entities/user_info_entity.dart';

abstract class AuthRepository {
  Future<Result<AuthTokensEntity>> login({
    required String usernameOrEmail,
    required String password,
  });

  Future<Result<AuthTokensEntity>> refreshToken({
    required String refreshToken,
  });

  Future<Result<UserInfoEntity>> fetchMe({
    required String accessToken,
  });
}
