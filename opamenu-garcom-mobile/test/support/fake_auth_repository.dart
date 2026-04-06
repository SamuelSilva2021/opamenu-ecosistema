import 'package:opamenu_garcom_mobile/src/core/domain/result/result.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/entities/auth_tokens_entity.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/entities/user_info_entity.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/domain/repositories/auth_repository.dart';

class FakeAuthRepository implements AuthRepository {
  Result<AuthTokensEntity>? nextLoginResult;
  Result<UserInfoEntity>? nextMeResult;

  @override
  Future<Result<AuthTokensEntity>> login({
    required String usernameOrEmail,
    required String password,
  }) async {
    return nextLoginResult as Result<AuthTokensEntity>;
  }

  @override
  Future<Result<UserInfoEntity>> fetchMe({required String accessToken}) async {
    return nextMeResult as Result<UserInfoEntity>;
  }
}

