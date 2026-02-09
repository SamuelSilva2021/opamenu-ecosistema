
import 'package:fpdart/fpdart.dart';
import 'package:opamenu_gestor/features/users/domain/models/user_model.dart';
import 'package:opamenu_gestor/features/users/domain/models/role_model.dart';

abstract class UserRepository {
  Future<Either<String, List<UserModel>>> getUsers();
  Future<Either<String, List<RoleModel>>> getRoles();
  Future<Either<String, void>> createUser(Map<String, dynamic> data);
  Future<Either<String, void>> updateUser(String id, Map<String, dynamic> data);
  Future<Either<String, void>> deleteUser(String id);
}
