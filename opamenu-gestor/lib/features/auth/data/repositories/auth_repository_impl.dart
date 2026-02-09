
import 'package:fpdart/fpdart.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_remote_datasource.dart';
import '../models/login_request_model.dart';
import '../models/login_response_model.dart';
import '../models/user_info_model.dart';

part 'auth_repository_impl.g.dart';

@riverpod
AuthRepository authRepository(Ref ref) {
  return AuthRepositoryImpl(ref.watch(authRemoteDataSourceProvider));
}

class AuthRepositoryImpl implements AuthRepository {
  final AuthRemoteDataSource _dataSource;

  AuthRepositoryImpl(this._dataSource);

  @override
  Future<Either<String, LoginResponseModel>> login(
      LoginRequestModel request) async {
    try {
      final result = await _dataSource.login(request);
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, LoginResponseModel>> refreshToken(String refreshToken) async {
    try {
      final result = await _dataSource.refreshToken(refreshToken);
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, UserInfoModel>> getUserInfo({String? token}) async {
    try {
      final result = await _dataSource.getUserInfo(token: token);
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }
}
