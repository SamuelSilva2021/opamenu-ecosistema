import '../../../../core/domain/result/result.dart';
import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../domain/entities/auth_tokens_entity.dart';
import '../../domain/entities/user_info_entity.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_remote_data_source_contract.dart';
import '../models/auth_tokens_model.dart';
import '../models/user_info_model.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthRemoteDataSourceContract remoteDataSource;

  const AuthRepositoryImpl(this.remoteDataSource);

  @override
  Future<Result<AuthTokensEntity>> login({
    required String usernameOrEmail,
    required String password,
  }) async {
    final result = await remoteDataSource.login(
      usernameOrEmail: usernameOrEmail,
      password: password,
    );

    if (result is FailureResult<AuthTokensModel>) {
      return FailureResult(result.failure);
    }

    final model = (result as SuccessResult<AuthTokensModel>).value;
    return SuccessResult(model.toEntity());
  }

  @override
  Future<Result<UserInfoEntity>> fetchMe({required String accessToken}) async {
    final result = await remoteDataSource.me(accessToken: accessToken);
    if (result is FailureResult<UserInfoModel>) {
      return FailureResult(result.failure);
    }

    final model = (result as SuccessResult<UserInfoModel>).value;
    return SuccessResult(model.toEntity());
  }
}
