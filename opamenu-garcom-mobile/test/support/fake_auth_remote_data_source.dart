import 'package:opamenu_garcom_mobile/src/core/domain/result/result.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/data/datasources/auth_remote_data_source_contract.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/data/models/auth_tokens_model.dart';
import 'package:opamenu_garcom_mobile/src/features/auth/data/models/user_info_model.dart';

class FakeAuthRemoteDataSource implements AuthRemoteDataSourceContract {
  Result<AuthTokensModel>? nextLoginResult;
  Result<UserInfoModel>? nextMeResult;

  @override
  Future<Result<AuthTokensModel>> login({
    required String usernameOrEmail,
    required String password,
  }) async {
    return nextLoginResult as Result<AuthTokensModel>;
  }

  @override
  Future<Result<UserInfoModel>> me({required String accessToken}) async {
    return nextMeResult as Result<UserInfoModel>;
  }
}

