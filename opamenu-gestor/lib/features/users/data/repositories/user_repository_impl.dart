
import 'package:fpdart/fpdart.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:opamenu_gestor/features/users/domain/models/user_model.dart';
import 'package:opamenu_gestor/features/users/domain/models/role_model.dart';
import '../../domain/repositories/user_repository.dart';
import '../datasources/users_remote_datasource.dart';

part 'user_repository_impl.g.dart';

@riverpod
UserRepository userRepository(Ref ref) {
  final dataSource = ref.watch(usersRemoteDataSourceProvider);
  return UserRepositoryImpl(dataSource);
}

class UserRepositoryImpl implements UserRepository {
  final UsersRemoteDataSource _dataSource;

  UserRepositoryImpl(this._dataSource);

  @override
  Future<Either<String, List<UserModel>>> getUsers() async {
    try {
      final result = await _dataSource.getUsers();
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, List<RoleModel>>> getRoles() async {
    try {
      final result = await _dataSource.getRoles();
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, void>> createUser(Map<String, dynamic> data) async {
    try {
      await _dataSource.createUser(data);
      return const Right(null);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, void>> updateUser(String id, Map<String, dynamic> data) async {
    try {
      await _dataSource.updateUser(id, data);
      return const Right(null);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, void>> deleteUser(String id) async {
    try {
      await _dataSource.deleteUser(id);
      return const Right(null);
    } catch (e) {
      return Left(e.toString());
    }
  }
}
