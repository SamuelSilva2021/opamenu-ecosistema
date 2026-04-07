import '../../../../core/domain/result/result.dart';
import '../models/auth_tokens_model.dart';
import '../models/user_info_model.dart';

abstract class AuthRemoteDataSourceContract {
  Future<Result<AuthTokensModel>> login({
    required String usernameOrEmail,
    required String password,
  });

  Future<Result<AuthTokensModel>> refreshToken({
    required String refreshToken,
  });

  Future<Result<UserInfoModel>> me({required String accessToken});
}
